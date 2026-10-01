# FrameworkOnCore

.NET Framework の Web アプリ(ASP.NET Web Forms、C# と VB)を、**ほぼそのままの形で .NET 10 に移し、Linux で動かす**ための道具です。

- ソースとマークアップはできるだけ元のまま使います。変換器はプロジェクトファイルと、.NET にない API の呼び出しだけを書き換えます。
- System.Web には [WebFormsForCore](https://github.com/webformsforcore/WebFormsForCore)(ASP.NET Web Forms を ASP.NET Core 上で動かすもの)を使います。リポジトリの `FrameworkOnCore.Runtime/` に取り込み、FrameworkOnCore で手を入れて保守しています。
- .NET Framework にしかない API の扱いは、部品ごとに利用者が選べます(対応しない、を含む)。

> 以前の「Web Forms を Blazor に変換する」方針(WebForm2Blazor)から 2026-09-26 に切り替えました。旧方針のコードと資料は 2026-09-30 にリポジトリから消しました(git の履歴にあります)。

## 全体の流れ

```
 .NET Framework のアプリ
        │
        ▼
 ① 解析(analyze / Studio)  … 使っている .NET Framework の API と回数、.NET 10 での状態
        │
        ▼
 ② 部品ごとの選択(Studio → foc-choices.json)
        │
        ▼
 ③ 変換(FrameworkOnCore.Converter --choices)  … SDK 形式のプロジェクト、API の書き換え、ビルド
        │
        ▼
 ④ 配置(Dockerfile / systemd)  … Linux で起動、ファイル名の大文字小文字は Windows と同じ扱い
```

## 構成

| パス | 内容 |
|---|---|
| `src/FrameworkOnCore.Converter` | 変換器(CLI)。古い形式のプロジェクトを .NET 10 の SDK 形式にし、ビルドのエラーをもとにソースを直す。元のビルド(`--build-original`)、配置の出力も行う。`analyze` で解析だけも行える |
| `src/FrameworkOnCore.Analysis` | 解析エンジン(ライブラリ)。ソースを .NET Framework 4.8 の参照アセンブリでコンパイルし、API ごとの回数と場所、.NET 10 での状態、部品を出す。カタログ(`catalog/components.json`)と選択(`Choices`)もここ |
| `src/FrameworkOnCore.Studio` | GUI(ローカルの Web 画面)。解析、部品ごとの選択、ソースの表示、変換のコマンド |
| `src/FrameworkOnCore.Analyzers` | 変換のビルドで使う Roslyn アナライザー(Windows のパス、非同期デリゲート、プラットフォームの置き換え) |
| `experiments/wf4c/shims/FrameworkOnCore.Compat` | 互換アセンブリ。.NET にない、または Windows 専用のものを .NET Framework と同じ動きで補う(EventLog、Encoding.Default、Thread.ResetAbort、VB の My など) |
| `FrameworkOnCore.Runtime` | 変換後のアプリが動く土台(System.Web など)。FrameworkOnCore で保守している WebFormsForCore で、上流を git subtree で取り込んだもの(中のプロジェクトは上流の名前 `src/WebFormsForCore.*` のまま、パッケージ名は `FrameworkOnCore.*`。Ajax Control Toolkit も `src/WebFormsForCore.AjaxControlToolkit` に同じく取り込み済み)。上流への変更は取り込みの後のコミット(以前のパッチ 0001〜0040 に当たる。0032・0033 は LINQ to SQL、0034〜0037 は System.Drawing の移植、0038 は Linux での生成物のフォルダーの掃除、0039・0040 は System.Web.DataVisualization(グラフ)の移植。その後の System.Web.Mobile の移植はこのリポジトリのコミット)。上流の更新は `git subtree pull --prefix=FrameworkOnCore.Runtime https://github.com/webformsforcore/WebFormsForCore.git main` |
| `experiments/wf4c/casefs` | Linux でファイル名の大文字小文字を区別しない LD_PRELOAD ライブラリ(libfoccase.so) |
| `tests/FrameworkOnCore.Tests` | テスト(Windows と Linux) |
| `tests/DataLinqParity` | 移植した System.Data.Linq の全 API の新旧比較のケース(net48 で .NET Framework のゴールデンを採り、テストが移植版と比べる) |
| `tests/DrawingParity` | 移植した System.Drawing の全 API の新旧比較のケース(同じ方法。Windows は完全一致、Linux は既知の差分を理由つきで `known-differences.json` に持つ) |
| `tests/DataVisualizationParity` | 移植した System.Web.DataVisualization(グラフ)の全 API の新旧比較のケース(同じ方法。グラフを描くシナリオも含む) |
| `tests/MobileParity` | 移植した System.Web.Mobile(モバイル コントロール)の全 API の新旧比較のケース(同じ方法) |
| `tests/Parity.Core` | 新旧比較の共通部分(観測値の書き方、ケースの実行、ゴールデンの書き出し、API の一覧から型とメンバーごとにケースを作る仕組み) |
| `samples/` | 検証用の小さな Web Forms アプリ(RuntimeProbe、グラフの ChartProbe、モバイル コントロールの MobileProbe、Ajax Control Toolkit の ToolkitScriptManager の ToolkitProbe など) |
| `corpora/` | 実在の OSS アプリ(コーパス)の取得スクリプトと記録。本体は取得して使う(リポジトリには含めない) |

## 準備

- .NET 10 SDK
- Windows(変換器は Visual Studio の MSBuild で元のアプリをビルドする)。Linux での検証には Docker Desktop

FrameworkOnCore のパッケージ(`FrameworkOnCore.*`)は、初めて解析や変換をしたときに、このリポジトリの GitHub Release から `experiments/wf4c/_feed` に自動で取得されます。互換アセンブリも必要なときに自動でビルドされます。事前の準備は要りません(リポジトリが非公開の間は、GitHub CLI にログインしているか、`GH_TOKEN` を設定しておく必要があります)。

## 使い方

### Studio(GUI)

```powershell
dotnet build src\FrameworkOnCore.Studio
dotnet src\FrameworkOnCore.Studio\bin\Debug\net10.0\FrameworkOnCore.Studio.dll
# http://127.0.0.1:5300/ を開く(localhost だけで待ち受ける)
```

「新しい解析」で .NET Framework の Web プロジェクト(.csproj / .vbproj)を指定すると、解析結果が出ます。部品ごとに対応を選んで保存し、「変換してビルド」を押すと、その選択で変換とビルドを行います。ビルドできたら、Linux に配置できる形(Dockerfile と systemd 用のスクリプト付き、`obj` を除く)を ZIP でダウンロードできます。「Linux(Docker)で起動」で、その Dockerfile からイメージを作り、コンテナを localhost のポートで起動して確かめることもできます(Docker Desktop が要ります。接続文字列などは環境変数で渡します。Studio のコンテナは一度に一つ)。コマンドラインで変換するためのコマンドもコピーできます。

### コマンドライン

```powershell
$converter = 'src\FrameworkOnCore.Converter\bin\Debug\net10.0\FrameworkOnCore.Converter.dll'
dotnet build src\FrameworkOnCore.Converter

# 解析だけ(api-analysis.json、API-ANALYSIS.md、既定の選択の foc-choices.json)
dotnet $converter analyze <Web プロジェクト> --out <出力先>

# 変換(選択は省略可。省略すると既定)
dotnet $converter <Web プロジェクト> --out <出力先> --choices foc-choices.json [--build-original] [--culture-profile <file>]
```

変換の結果は `<出力先>\CONVERSION-REPORT.md` にまとまります。Linux への配置は `<出力先>\Dockerfile` と `deploy/`(`deploy/README.md`)を使います。

### テスト

```powershell
dotnet test tests\FrameworkOnCore.Tests
.\tests\FrameworkOnCore.Tests\run-tests-linux.ps1    # Linux(Docker)
```

### `FrameworkOnCore.Runtime/` を変えるとき(開発者向け)

ソースは `FrameworkOnCore.Runtime/` です。直したら `experiments/wf4c/pack-frameworkoncore.ps1 -Build All` で `_feed` にパッケージを作ります(`_feed` にある版は取得されません)。配るときは版を上げ(`pack-frameworkoncore.ps1 -Version` の既定と `rules/packages.json` の `frameworkOnCoreVersion`)、push します。GitHub Actions(`.github/workflows/frameworkoncore-packages.yml`)が同じ手順でビルドし、Linux でテストして、まだ無い版なら GitHub Release に置きます(手で動かすときは Actions の「FrameworkOnCore packages」の Run workflow。publish にチェックを入れたときだけ公開)。手元から置くときは `experiments/wf4c/publish-frameworkoncore.ps1`。

## 状態

- コーパス(Web Forms: BlogEngine.NET、WingtipToys、mojoPortal、YAF.NET、DNN、N2CMS、openIMIS(VB)、nopCommerce 1.90。MVC 5: MvcMovie、nopCommerce 3.90)を変換し、Windows と Linux で動作を確かめています。結果と既知の課題は `experiments/wf4c/README.md` にあります。
- .NET Framework にしかない API の洗い出しと対応の状況は `experiments/wf4c/FRAMEWORK-ONLY-APIS.md` にあります。
- LINQ to SQL(System.Data.Linq)は referencesource(MIT)から `FrameworkOnCore.Runtime/` に移植済みで、既定で使われる。全 API(486)を .NET Framework と新旧比較して一致を確認している(`experiments/wf4c/README.md` の記録)。
- System.Drawing は dotnet/runtime 6.0 の System.Drawing.Common(Linux 実装を持つ最後の版、MIT)を `FrameworkOnCore.Runtime/` に移植済みで、Windows は GDI+、Linux は libgdiplus で動く。全 API(3,354 のうち .NET にある 3,185 と、.NET が足した 40)を .NET Framework と新旧比較している(`experiments/wf4c/README.md` の記録)。
- グラフ(System.Web.DataVisualization の Chart コントロール)は referencesource(MIT)から `FrameworkOnCore.Runtime/` に移植済みで、既定で使われる。全 API(1,661)を .NET Framework と新旧比較し、ChartImg.axd を使うサイト(`samples/ChartProbe`)を IIS と Windows・Linux で比べている(`experiments/wf4c/README.md` の記録)。
- モバイル コントロール(System.Web.Mobile)は referencesource(MIT)から `FrameworkOnCore.Runtime/` に移植済みで、既定で使われる。全 API(1,819)を .NET Framework と新旧比較し、モバイル コントロールのサイト(`samples/MobileProbe`)を IIS と Windows・Linux で比べている(`experiments/wf4c/README.md` の記録)。
- 予定の選択肢: WCF のサービスを CoreWCF で動かす。

## ドキュメント

| ファイル | 内容 |
|---|---|
| `LINUX-CONVERTER-DESIGN.md` | 変換器の設計 |
| `experiments/wf4c/README.md` | 実験と検証の記録(WebFormsForCore、コーパス、Linux、解析、Studio) |
| `experiments/wf4c/FRAMEWORK-ONLY-APIS.md` | .NET Framework にしかない API の一覧と対応 |
| `corpora/README.md` | コーパスの取得と記録 |
