# 検証用コーパス

FrameworkOnCore の回帰検証に使うコーパスです。実在のアプリ（BlogEngine.NET、WingtipToys など）は「そのアプリが使っている機能」しか確かめられず、1 本動かすのに時間がかかるわりに、観点が偏ります。このコーパスは、確かめたい観点ごとに作った小さな .NET Framework の Web アプリ（プローブ）の集まりで、次のことを一度に確かめます。

1. **変換**: 変換器が、利用者と同じ手順でプローブを変換してビルドできること。変換の判断（プロジェクトの設定、パッケージ、レポートの項目、追加・変更したファイル）が基準（`baseline/<プローブ>.json`）と同じであること。
2. **動作**: 変換後のアプリを Windows と Linux で動かし、各ページの答えが、変換前のアプリを IIS で動かしたときの答え（`samples/<プローブ>/golden-webforms.json`）と同じであること。

あわせて、ホストのテストのうち自動生成のもの（IIS の設定 1 つずつ、攻撃・異常入力、IIS のパイプライン）も観点として一覧に載せています（`ASPECTS.md`）。

## 構成

| ファイル | 内容 |
|---|---|
| `probes.json` | コーパスに入れるプローブの一覧（`samples/<名前>`） |
| `catalog.py` | 観点の一覧 `ASPECTS.md` を作り、`--check` でコーパスに欠けがないか確かめる |
| `ASPECTS.md` | 観点の一覧（自動生成。編集しない） |
| `run.ps1` | 全プローブを変換・実行・比較し、`_out/RESULTS-<windows|linux>.md` に結果を書く |
| `baseline/` | 各プローブの変換の判断の基準 |

1 つのプローブの処理は `experiments/wf4c/sample-parity.ps1` が行います（変換、DB の初期化、Windows または Linux のコンテナでの実行、ParityTest での比較）。`run.ps1` はそれを全プローブ分、順に呼びます。

## 実行

```powershell
.\corpus\run.ps1                 # Windows（32 プローブで約 1 時間）
.\corpus\run.ps1 -Linux          # Linux（コンテナは 1 つずつ。終わったら Docker を止め wsl --shutdown）
.\corpus\run.ps1 -Only PageProbe,AuthProbe
.\corpus\run.ps1 -UpdateBaseline # 変換の判断の差を確かめたうえで、基準を記録し直す
```

- DB を使うプローブ（`samples/<名前>/database.json`）があるので、SQL Server Express を起動してから実行し、終わったら止めます。
- プローブは、正解を記録したときと同じポート（5096）で 1 つずつ動かします。URL を表示するページがあるためです。
- 変換器は、プローブにカルチャの指定（`cultures.txt`）がなければこの PC の設定を取り込みます。シェルに `DOTNET_CLI_UI_LANGUAGE` などがあると UI 言語が変わるので、`run.ps1` は実行前にそれらを外します。

## 観点の足し方（継続的に足していく）

観点は「不具合を直したとき」「IIS や .NET Framework の動きを新しく確かめたとき」「新しい API や設定に対応したとき」に足します。**直した不具合には、それを捕まえる観点を必ず 1 つ足す**のが原則です。

1. **既存のプローブに足すか、新しいプローブを作るか決める**: 同じ種類の観点（ページの仕組み、認証、キャッシュ、ロケール、URL Rewrite…）は既存のプローブに足します。ルートの web.config や Global.asax が他の観点と衝突するものだけ、新しいプローブ（`samples/<名前>Probe`）にします。プローブを増やすほど実行時間が延びるので、1 つのプローブに多くの観点を詰めます。
2. **ページを足す**: 観点を確かめるページ（`.aspx`、ハンドラーなど）を作り、確かめたい値をテキストで出します。IIS と移植側で同じになるべき値だけを出します（時刻や乱数は出さない）。
3. **シナリオに手順を足す**: `samples/<プローブ>/parity-scenario.json` に、ページを開く・操作する手順と `snapshot`（名前は観点が分かるもの）を足します。シナリオ冒頭の `_comment` に、何を確かめるプローブかを書き足します。
4. **IIS で正解を記録し直す**: 管理者の PowerShell で `.\experiments\wf4c\sample-parity.ps1 -Name <プローブ> -Record`（変換前のアプリを IIS で動かし、`golden-webforms.json` に記録します）。
5. **一覧と基準を更新する**: `python corpus/catalog.py --check` で欠けがないことを確かめ、`ASPECTS.md` を更新します。新しいプローブは `probes.json` に足し、`.\corpus\run.ps1 -Only <プローブ> -UpdateBaseline` で基準を記録します。
6. **ホスト層（IIS の機能）の観点**は、ホストの自動生成テストに足すほうが効率的です。設定は `tests/RuntimeUnitTests/Security/Settings/SettingsSamples.cs`（値と文脈）、要求は `SettingsSite.cs` の `SettingsProbes`、異常入力は `Attack/AttackCases.cs` に足し、`iis-golden/settings.ps1`・`record.ps1` で IIS に記録し直します。HTTPS の要求（クライアント証明書あり・なし）も `SettingsProbes` にあり、証明書は記録のたびに作って後で消します（`SettingsCertificates`）。アプリのルートにしか書けない設定（`modules` など）は `SettingsSamples.ApplicationSections` に足すと、1 つずつ別のアプリとして試されます。

`catalog.py --check` は、`probes.json` にないサンプル、正解のない `snapshot`、同じ名前の `snapshot` を見つけると失敗します。観点の足し忘れや記録し忘れを防ぐためです。
