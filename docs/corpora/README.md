# コーパスで変換を試す

実在のオープンソースの .NET Framework の Web アプリ(コーパス)を、GitHub から取って FrameworkOnCore で変換し(コマンドラインか Studio)、Windows と Linux のコンテナで動かすまでの手順です。

1. [共通の準備](setup.md) をします(一度だけ)。
2. 試すコーパスのページを開きます。最初は **[be](be.md)** をお勧めします(SQL Server も元のアプリのビルドも要らず、準備がいちばん少ないため)。

| 名前 | アプリ | 種類 | 追加で要るもの | 結果 |
|---|---|---|---|---|
| [be](be.md) | BlogEngine.NET 3.3.8 | Web Forms | なし | 動く(**確認** 2026-11-05) |
| [wt](wt.md) | WingtipToys | Web Forms | Docker(SQL Server) | 動く(**確認** 2026-11-05) |
| [mvcmovie](mvcmovie.md) | MvcMovie | MVC 5 | Docker(SQL Server) | 12 画面中 11 が元と同じ |
| [contoso](contoso.md) | Contoso University | MVC 5 | Docker(SQL Server) | 12 画面中 9 が元と同じ |
| [nop](nop.md) | nopCommerce 1.90 | Web Forms | Build Tools | 動く |
| [nop390](nop390.md) | nopCommerce 3.90 | MVC 5 | Build Tools | 動く |
| [mojo](mojo.md) | mojoPortal 3.1.6 | Web Forms | Build Tools | 動く |
| [n2](n2.md) | N2 CMS | Web Forms | Build Tools | 動く |
| [imis](imis.md) | openIMIS 24.10 | Web Forms(VB) | Build Tools | 動く |
| [dnn](dnn.md) | DNN Platform 9.13.10 | Web Forms | Build Tools、ディスク 20 GB | 動く |
| [yaf](yaf.md) | YAF.NET 3.2.15 | Web Forms | なし | 動かない(変換まで) |

「確認」と書いたものは、この手順のとおり(コマンドラインと Studio、Windows と Linux)に動かして確かめました。それ以外は、検証の記録(`experiments/wf4c/README.md`)にある結果で、それより後に出たパッケージでは確かめていません。
