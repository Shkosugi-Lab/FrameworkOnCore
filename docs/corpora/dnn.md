# dnn(DNN Platform 9.13.10)

大きな CMS です(Web Forms、C# と VB、SQL Server)。[共通の準備](setup.md) が終わっている前提です。

| | |
|---|---|
| 取得元 | [dnnsoftware/Dnn.Platform](https://github.com/dnnsoftware/Dnn.Platform) タグ `v9.13.10` |
| フォルダー | `corpora\work\Dnn.Platform-9.13.10` |
| ライセンス | MIT |
| 追加で要るもの | Build Tools、SQL Server Express と `sqlcmd`。ディスク約 20 GB |

## 1. GitHub から取得して変換する

元のアプリをビルド(DNN の Cake のビルド)してから変換します。初回は長くかかります。

```powershell
cd corpora\work
curl.exe -L -o dnn.zip https://github.com/dnnsoftware/Dnn.Platform/archive/refs/tags/v9.13.10.zip
tar -xf dnn.zip          # Dnn.Platform-9.13.10 ができる
cd ..\..
Start-Service 'MSSQL$SQLEXPRESS'                              # 管理者の PowerShell で
.\experiments\wf4c\convert-corpora.ps1 -Only dnn
```

`the build failed after it deployed the site: ... is used as it is` と出ても、そのまま進めて構いません(ビルドの一部が失敗しただけで、サイトはできています)。

## 2. Linux で動かす

サイトと SQL Server をコンテナで動かし、空の DB `dnn_linux` にインストールしてから、トップを開きます。

```powershell
.\experiments\wf4c\run-linux-site.ps1 -Site dnn\site -Dll DotNetNuke.Website -Database dnn_linux -Paths '/Install/Install.aspx?mode=install', '/' -CaseInsensitive -Keep
```

- 確認: インストールが終わり、トップが 200 になります(記録 2026-10-11)。
- そのまま http://localhost:5098/ を開けます(`/Login`、`/Terms`、`/Privacy` も開けます)。

## 3. 片付け

```powershell
docker rm -f w2l-site-dnn w2l-sql
```
