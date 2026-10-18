// The page is built into the server's wwwroot (Studio serves it as static files): fixed names (app.js, app.css), the
// fonts beside them, so that a build changes only what changed.
import { defineConfig } from 'vite';
import react from '@vitejs/plugin-react';

export default defineConfig({
  plugins: [react()],
  base: './',
  build: {
    outDir: '../wwwroot',
    emptyOutDir: true,
    chunkSizeWarningLimit: 4096,
    rollupOptions: {
      output: {
        entryFileNames: 'app.js',
        chunkFileNames: '[name].js',
        assetFileNames: assetInfo => (assetInfo.names?.[0] ?? '').endsWith('.css') ? 'app.css' : 'assets/[name][extname]',
      },
    },
  },
  server: { proxy: { '/api': 'http://127.0.0.1:5300' } },
});
