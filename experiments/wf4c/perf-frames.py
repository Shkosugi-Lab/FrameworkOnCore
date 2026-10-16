"""The busiest frames of a speedscope trace (dotnet-trace --format speedscope, perf-check.ps1 -Trace), in CPU time only:
dotnet-trace's stacks end with a pseudo frame, CPU_TIME while the thread runs (UNMANAGED_CODE_TIME and the waits while
it does not). Each frame's CPU time on the top of the stack (exclusive: the frame under CPU_TIME) and on the stack
(inclusive), over every thread.
    python perf-frames.py <trace.speedscope.json> [top] [filter]"""
import json
import sys
from collections import defaultdict

path = sys.argv[1]
top = int(sys.argv[2]) if len(sys.argv) > 2 else 30
only = sys.argv[3] if len(sys.argv) > 3 else None
trace = json.load(open(path, encoding='utf-8'))
frames = [f['name'] for f in trace['shared']['frames']]
cpu = {i for i, name in enumerate(frames) if name == 'CPU_TIME'}
exclusive = defaultdict(float)
inclusive = defaultdict(float)
total = 0.0

for profile in trace['profiles']:
    if profile.get('type') != 'evented':
        continue
    stack = []
    last = None
    for event in profile['events']:
        at = event['at']
        if last is not None and len(stack) > 1 and stack[-1] in cpu:
            span = at - last
            total += span
            exclusive[stack[-2]] += span
            for frame in set(stack[:-1]):
                inclusive[frame] += span
        last = at
        if event['type'] == 'O':
            stack.append(event['frame'])
        elif stack:
            stack.pop()


def name(index):
    text = frames[index]
    return text if len(text) <= 160 else text[:157] + '...'


print('CPU time %.0f ms' % total)
print('--- exclusive')
for f, t in sorted(exclusive.items(), key=lambda x: -x[1])[:top]:
    print('%8.0f ms %5.1f%%  %s' % (t, 100 * t / total, name(f)))
print('--- inclusive' + (' (' + only + ')' if only else ''))
rows = [(t, f) for f, t in inclusive.items() if only is None or only in frames[f]]
for t, f in sorted(rows, reverse=True)[:top]:
    print('%8.0f ms %5.1f%%  %s' % (t, 100 * t / total, name(f)))

# perf-frames.py <trace> <top> <filter> --callers <frame text>: the stacks below a frame (its callers, 4 deep), by CPU time.
if '--callers' in sys.argv:
    target = sys.argv[sys.argv.index('--callers') + 1]
    callers = defaultdict(float)
    for profile in trace['profiles']:
        if profile.get('type') != 'evented':
            continue
        stack = []
        last = None
        for event in profile['events']:
            at = event['at']
            if last is not None and len(stack) > 1 and stack[-1] in cpu:
                for depth in range(len(stack) - 2, -1, -1):
                    if target in frames[stack[depth]]:
                        key = ' <- '.join(name(f).split('!')[-1][:90] for f in reversed(stack[max(0, depth - 4):depth]))
                        callers[key] += at - last
                        break
            last = at
            if event['type'] == 'O':
                stack.append(event['frame'])
            elif stack:
                stack.pop()
    print('--- callers of ' + target)
    for key, t in sorted(callers.items(), key=lambda x: -x[1])[:12]:
        print('%8.0f ms  %s' % (t, key))
