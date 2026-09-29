# コーパス

変換器は、実在する .NET Framework の Web アプリ(コーパス)を変換し、Windows と Linux で元と同じに動くかで確かめます。
その取得と、元のアプリの描画の記録をここで扱います。

コーパス本体はサードパーティのソースで、それぞれ独自のライセンスを持ちます。
リポジトリには含めず、`fetch.ps1` が既知のバージョンを `corpora/work/` に取得します(`.gitignore` 済み)。

## 対象

| 名前 | コーパス | 取得元 |
|---|---|---|
| `be` | BlogEngine.NET 3.3.8.0 | `rxtur/BlogEngine.NET` @ `v3.3.8.0` |
| `mojo` | mojoPortal 3.1.6 | `i7MEDIA/mojoportal` @ `v3.1.6` |
| `yaf` | YAF.NET 3.2.15 | `YAFNET/YAFNET` @ `v3.2.15` |
| `dnn` | DNN Platform 9.13.10 | `dnnsoftware/Dnn.Platform` @ `v9.13.10` |
| `n2` | n2cms | `n2cms/n2cms` @ `master` |
| `wt` | WingtipToys | `corn-mendoza/wingtiptoys` @ `master` |
| `imis` | openIMIS Web Application(VB。24.10 で開発終了) | `openimis/web_app_vb` @ `main` |
| `imisdb` | openIMIS の SQL Server の DB(`imis` のデモの DB を作るスクリプト) | `openimis/database_ms_sqlserver` @ `24.10` |
| `nop` | nopCommerce 1.90(C#。最後の Web Forms 版、EF4) | `nopSolutions/nopCommerce` @ `release-1.90` |

日常の検証に使うのは `be` と `wt` です。ほかは必要なときに対象を指定して使います。

## 使い方

```powershell
.\corpora\fetch.ps1                                   # 取得(初回のみ)
.\experiments\wf4c\analyze-corpora.ps1 -Only be,wt    # 解析(experiments\wf4c\_analysis\<名前>\)
.\experiments\wf4c\convert-corpora.ps1 -Only be,wt    # 変換(元のビルドを含む)
```

変換したアプリを Linux のコンテナで動かし、元のアプリの描画と比べるには `experiments\wf4c\run-linux.ps1` を使います。

```powershell
.\experiments\wf4c\run-linux.ps1 -App be\BlogEngine\BlogEngine.NET `
    -Scenario corpora\regression\be.scenario.json -Golden corpora\parity\be.golden-webforms.json
```

コーパスごとの結果と既知の差異は `experiments/wf4c/README.md` にあります。

## 元のアプリの描画(正解)

| パス | 内容 |
|---|---|
| `regression/<名前>.scenario.json` | 開くページと操作(ParityTest のシナリオ) |
| `parity/<名前>.golden-webforms.json` | 変換前のアプリを IIS / .NET Framework で動かして採った描画 |

採り直すときは、元のアプリをビルドしてから IIS で動かして採ります。

```powershell
.\corpora\build-original.ps1 -Only be          # Visual Studio 無しでビルド
.\corpora\record-webforms-golden.ps1 -Only be  # IIS で動かして採る
```

`record-webforms-golden.ps1` を実行するには、IIS と、`wt` では SQL Server Express(`.\SQLEXPRESS`)が必要です。設定の理由はスクリプトのコメントに書いてあります。
