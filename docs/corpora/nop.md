# nop(nopCommerce 1.90)

EC サイトです(Web Forms、C#、SQL Server)。[共通の準備](setup.md) が終わっている前提です。

| | |
|---|---|
| 取得元 | [nopSolutions/nopCommerce](https://github.com/nopSolutions/nopCommerce) タグ `release-1.90` |
| フォルダー | `corpora\work\nopCommerce-release-1.90` |
| ライセンス | nopCommerce Public License 1.0 |
| 追加で要るもの | Build Tools。ディスク約 2 GB |

## 1. GitHub から取得して変換する

元のアプリをビルドしてから変換します。

```powershell
cd corpora\work
curl.exe -L -o nop.zip https://github.com/nopSolutions/nopCommerce/archive/refs/tags/release-1.90.zip
tar -xf nop.zip          # nopCommerce-release-1.90 ができる
cd ..\..
.\experiments\wf4c\convert-corpora.ps1 -Only nop
```

## 2. Linux で動かす

サイトと SQL Server をコンテナで動かし、ブラウザーで nopCommerce のインストーラーを動かします。

```powershell
.\experiments\wf4c\run-linux-site.ps1 -Site nop\site -Dll NopCommerceStore -Paths '/install/install.aspx' -CaseInsensitive -Keep
```

http://localhost:5098/install/install.aspx を開き、次の値でインストールします。

| 欄 | 値 |
|---|---|
| SQL Server | `w2l-sql` |
| 認証 | SQL Server 認証。ユーザー `sa`、パスワード `W2l_local_Passw0rd` |
| データベース | 新しく作る(名前は例えば `nop_linux`)。サンプルのデータも作る |

インストールが終わると、店頭(`/`)と管理画面が開けます。管理者は `admin@yourstore.com` / `admin` です。

Linux で動くことは、すべての操作を元のアプリと比べるテストで確かめています(727 操作、記録 2026-11-03)。ただし、上のインストーラーを使う手順そのものは試していません。

## 3. 片付け

```powershell
docker rm -f w2l-site-nop w2l-sql
```
