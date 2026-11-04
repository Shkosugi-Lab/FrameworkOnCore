# be(BlogEngine.NET 3.3.8)

[共通の準備](setup.md) が終わっている前提です。コマンドは、とくに書いていなければリポジトリの一番上のフォルダーで実行します。
[コーパスの一覧に戻る](README.md)

## 1. 概要

- **どんなアプリ**: ブログのエンジン(Web Forms、C#)。データは DB ではなく `App_Data` の XML ファイル。Web API 2、Web Pages(Razor)のウィジェット、拡張機能(bin の DLL)を使います。
- **大きさ**: 取得 94 MB、変換の出力 0.6 GB。
- **特別なこと**: いちばん準備が少なく、**最初に試すのに向いています**(SQL Server も元のビルドも要りません)。ただし、Studio では「リポジトリのフォルダー」の指定が要ります(下の 3.2)。

## 2. 前提

共通の準備のほかに要るものはありません。

| もの | 要るか |
|---|---|
| SQL Server Express | 要らない |
| Visual Studio Build Tools(元のビルド) | 要らない |
| インストーラー | 無い |
| Edge | 比較(3.6・8)をするとき |
| Docker Desktop | Linux で動かすとき |
| ディスク | 取得 94 MB、変換の出力 0.6 GB。Linux のイメージ 1.15 GB |

## 3. 手順

この節の手順は、2026-11-04 にこのとおり実行して確かめました。

### 3.1 取得する

```powershell
.\corpora\fetch.ps1 -Only be
```

確認: 「すべて取得できました。」と出て、`corpora\work\BlogEngine.NET-3.3.8.0` ができます。

### 3.2 変換する

元のビルドは要りません(下の「5. 既知の差・制限」)。

**Studio で**([使い方](setup.md#41-studio画面)):

| 欄 | 値 |
|---|---|
| プロジェクト | `<リポジトリ>\corpora\work\BlogEngine.NET-3.3.8.0\BlogEngine\BlogEngine.NET\BlogEngine.NET.csproj` |
| リポジトリのフォルダー | **`<リポジトリ>\corpora\work\BlogEngine.NET-3.3.8.0`**(必ず指定します) |
| 構成 | `Release` |

be は `.sln` のフォルダーの外の `lib` の DLL を参照しています。「リポジトリのフォルダー」が空のままだと、変換でその参照が失われ、変換レポートの「未解決」が 16 件出ます。
解析は 10 秒ほど、変換は 1 分ほどです。ステップ 5 の「ネイティブ起動」では、ポート 8100 で起動しました。

**コマンドラインで(まとめて変換)**: 約 1 分。リポジトリのフォルダーはスクリプトが指定します。

```powershell
.\experiments\wf4c\convert-corpora.ps1 -Only be
```

確認: `=== be BUILD OK (rounds: 4) 未解決(変換で解決できなかったもの)(0 件) / ... / コンパイルのために変えたソース(スタブ・削除)(23 件) / ...` と `snapshot: the same as the baseline (936 entries)`。
出力は `experiments\wf4c\be`。

**コマンドラインで(変換器を直接)**: 約 1 分。

```powershell
.\converter.ps1 corpora\work\BlogEngine.NET-3.3.8.0\BlogEngine\BlogEngine.NET\BlogEngine.NET.csproj --out C:\work\be-out --root corpora\work\BlogEngine.NET-3.3.8.0 --build-original auto
```

確認: 最後に `build succeeded; web project: ...; report: C:\work\be-out\CONVERSION-REPORT.md`。

### 3.3 DB・インストーラー

ありません。データは `App_Data` の XML です。

### 3.4 Windows で動かす

```powershell
cd experiments\wf4c\be\BlogEngine\BlogEngine.NET
..\..\..\supervise.ps1 -Dll bin\BlogEngine.NET.dll -Port 5096
# ログはこのフォルダーの probe.log
```

**http://localhost:5096/** を開きます。最初の要求はページのコンパイルで 10 秒ほどかかります。止め方は [共通の準備の 6.2](setup.md#62-止める)。

### 3.5 開くページとログイン

| ページ | 見えるもの |
|---|---|
| `/` | タイトル「Name of the blog \| Short description of the blog」 |
| `/post/welcome-to-blogengine-net` | 最初の投稿 |
| `/archive` | 投稿の一覧 |
| `/page/Example-page` | 固定ページ |
| `/Account/login.aspx` | ログイン |
| `/admin/` | ログイン画面へ転送される |

管理者は BlogEngine の既定の **`admin` / `admin`** です。

### 3.6 元のアプリと比べる(Windows)

`supervise.ps1` を止めてから、リポジトリの一番上に戻って実行します(起動から停止までスクリプトが行います)。約 2 分。

```powershell
.\experiments\wf4c\verify-windows.ps1 -Name be
```

確認: `RESULT: OK (5 スナップショットすべて変換前と一致、リンク 17 件も一致)`。

### 3.7 Linux で動かす

Docker Desktop を起動しておきます。

**正解と比べる(`run-linux.ps1`)**: 約 3 分。

```powershell
.\experiments\wf4c\run-linux.ps1 -App be\BlogEngine\BlogEngine.NET -Scenario corpora\regression\be.scenario.json -Golden corpora\parity\be.golden-webforms.json
```

確認: `RESULT: OK (5 スナップショットすべて変換前と一致、リンク 17 件も一致)`。

**変換が書いた Dockerfile で動かす**: イメージ作りは約 1 分(2026-11-04)。

```powershell
cd experiments\wf4c\be
docker build -t blogengine-net .
docker run -d -p 8080:8080 --name blogengine-net -v blogengine-net-data:/app/App_Data blogengine-net
```

- **http://localhost:8080/** を開きます。
- 確認: `docker logs blogengine-net` に `Now listening on: http://[::]:8080` が出ていれば起動しています。
- Linux でもファイル名の大文字・小文字を区別しません(既定)。be では `/POST/Welcome-To-BlogEngine-NET` も正しいページへ転送されました。
- カルチャ(`LANG=ja_JP.UTF-8`)とタイムゾーン(`TZ=Asia/Tokyo`)は、カルチャのデータ(元のサーバーのもの)から Dockerfile に書かれています。
- `App_Data` はボリューム `blogengine-net-data` に置かれます。

## 4. 期待される結果

| 何を | 結果 | 出どころ |
|---|---|---|
| 変換 | `BUILD OK`。変換レポート: 未解決 0、スタブ 23(LINQ to SQL のファイル システム、WCF Data Services のギャラリー、デザイナー用の式エディターなど。既定の構成では使われません) | **確認** 2026-11-04 |
| Windows の画面の比較 | `RESULT: OK (5 スナップショットすべて変換前と一致、リンク 17 件も一致)` | **確認** 2026-11-04 |
| Linux の画面の比較 | 同じ(5/5、17/17) | **確認** 2026-11-04 |
| すべての操作の比較 | 操作 201 件(実行できないもの 4 件)。Windows は一致 180・許容 17、Linux は一致 170・許容 27。どちらも `RESULT: OK` | 記録 2026-11-02(w2l.27) |

## 5. 既知の差・制限

- 元のアプリのソリューションのビルド(`converter.ps1 build-original`)は BlogEngine.Core のビルドで失敗しますが、リポジトリに入っている `bin` をそのままサイトとして使うので、変換には影響しません(元のビルドは不要です)。
- すべての操作の比較で許される差: 文言(元のサーバーの .NET Framework の日本語の言語パックと、FrameworkOnCore の訳)、IIS の詳細エラー ページ(簡略版で書く)、TLS。
  Linux だけ: タイムゾーンの一覧(IANA)、カルチャの名前(ICU)、改行、物理パス、プロフィール画像の PNG のバイト。
- 実行できない 4 件: メールの中身、インターネット上のギャラリーと更新が要るもの。

## 6. うまくいかないとき

- **変換レポートの「未解決」が 16 件出る**: Studio で「リポジトリのフォルダー」に `corpora\work\BlogEngine.NET-3.3.8.0` を指定し忘れています(3.2)。
- **画面が前と違う・比較の結果がおかしい**: アプリを動かすと `App_Data` が書き換わります(投稿、ログなど)。最初の状態に戻します。

  ```powershell
  .\corpora\be-app-data.ps1 -Target experiments\wf4c\be\BlogEngine\BlogEngine.NET\App_Data
  ```

  すべての操作の比較(`-Suite operations`)は、始める前にスクリプトが `App_Data` をリリースの状態に戻します。
- 共通のこと(ポート、Docker、比較の失敗など)は [共通の準備の 10](setup.md#10-うまくいかないとき)。

## 7. 片付け

```powershell
# Windows で起動したアプリ
Get-NetTCPConnection -LocalPort 5096 -State Listen -ErrorAction SilentlyContinue | ForEach-Object { Stop-Process -Id $_.OwningProcess -Force }
# Dockerfile で動かしたコンテナとボリューム
docker rm -f blogengine-net
docker volume rm blogengine-net-data
# App_Data を最初の状態に戻す(続けて使うとき)
.\corpora\be-app-data.ps1 -Target experiments\wf4c\be\BlogEngine\BlogEngine.NET\App_Data
```

そのあと、Docker Desktop を止めて `wsl --shutdown`、Edge を閉じます([共通の準備の 9](setup.md#9-後片付け))。

## 8. IIS の結果と比べる(任意)

画面の比較(3.6・3.7)のほかに、**すべての操作**(201 件)を元のアプリ(IIS で記録した正解)と比べられます。

```powershell
.\experiments\wf4c\verify-windows.ps1 -Name be -Suite operations     # 約 13 分
.\experiments\wf4c\run-linux.ps1 -App be\BlogEngine\BlogEngine.NET -Suite operations   # 約 14 分
```

- 正解: `corpora\parity\be.operations.golden.json`(473 スナップショット)、許される差 `be.operations.allowed.json`。結果は `experiments\wf4c\_results\be.operations.windows.{md,json}`。
- 期待される結果: Windows・Linux とも `RESULT: OK`(4. 期待される結果)。
- 詳しくは [tests/README.md](../../tests/README.md) の「4. コーパス」。
