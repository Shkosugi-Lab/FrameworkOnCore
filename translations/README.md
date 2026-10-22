# FrameworkOnCore の翻訳

FrameworkOnCore のアセンブリが自分で出す文言(例外のメッセージ、デザイン時の説明を含む)の、FrameworkOnCore 独自の翻訳です。

| アセンブリ | 件数 | 主な文言 |
|---|---:|---|
| System.Web | 3,369 | Calendar の見出し、Login・CreateUserWizard などのラベルとメッセージ、Wizard・GridView のボタン、Menu・TreeView の補助テキスト、Web パーツの動作名、エラーページの見出しと説明 |
| System.Web.Extensions | 578 | ASP.NET AJAX: UpdatePanel・ScriptManager の例外、MicrosoftAjax.js などのクライアント側のメッセージ、ListView・DataPager |
| System.Web.DataVisualization | 1,290 | Chart コントロールの例外と説明 |
| System.Web.DynamicData | 101 | Dynamic Data のフィールド テンプレートとフィルター |
| System.Web.Mobile | 495 | モバイル コントロール |
| System.Web.Services | 445 | ASMX Web サービス(ヘルプ ページを含む) |
| System.Web.ApplicationServices | 21 | メンバーシップ |
| System.Web.Optimization、WebGrease | 12、276 | バンドルと縮小(CSS・JavaScript のエラー) |
| System.Configuration.ConfigurationManager | 252 | 構成ファイルのエラー |
| System.Data.SqlClient | 407 | SQL Server への接続のエラー |
| System.Drawing.Common | 117 | 画像のエラー |
| WebFormsForCore.Serialization.Formatters | 67 | BinaryFormatter |
| System.ComponentModel.Annotations(.NET のもの) | 62 | DataAnnotations の検証メッセージ(「{0} は必須です。」など。MVC・Web Pages・Web Forms のモデル バインド) |

全部で 7,492 件(System.Web 3,369 件、ほか 4,123 件)。一覧と英語の原文の場所は `sources.json` にあります。

## なぜ独自に訳すのか

.NET Framework では、言語パック(日本語の Windows には普通入っている)があるサーバーで、UI カルチャがその言語のとき、これらの文言がその言語になっていました。
.NET には言語パックが無く、FrameworkOnCore の文言は英語(referencesource)だけです。

Microsoft の言語パックは、使用許諾(MICROSOFT SOFTWARE SUPPLEMENTAL LICENSE TERMS – .NET FRAMEWORK AND ASSOCIATED LANGUAGE PACKS FOR MICROSOFT WINDOWS
OPERATING SYSTEM)の 1.c で「Windows 以外のプラットフォームで動かすための配布」を禁じています。Linux で動かす FrameworkOnCore には使えないので、
英語の原文から独自に訳したものをここで管理します。

**訳すときに、Microsoft の言語パックの訳文(Windows の .NET Framework が出す日本語)を写したり参考にしたりしないでください。** 英語の原文から訳してください。
そのため、文言が元のサーバー(.NET Framework)の表示と同じとは限りません。

## 使われ方

- `<言語>\<アセンブリ>\<リソース>.<言語>.resx` は、そのアセンブリのパッケージのサテライト(`lib/net10.0/<言語>/<アセンブリ>.resources.dll`。
  例: FrameworkOnCore.Web の `lib/net10.0/ja/System.Web.resources.dll`)になります(FrameworkOnCore.Runtime の LICENSE と同じ MIT)。
  ビルドは `FrameworkOnCore.Runtime/src/FrameworkOnCore.Translations.targets`(各アセンブリのプロジェクトが読み込みます)。
- .NET 自身のアセンブリ(System.ComponentModel.Annotations)の訳は、サテライトだけを作るプロジェクト
  (`FrameworkOnCore.Runtime/src/WebFormsForCore.Satellites.ComponentModel.Annotations`。英語の原文は dotnet/runtime v10.0.0 の
  `Resources/Strings.resx`、MIT)で作り、FrameworkOnCore.Web のパッケージに入れます。.NET は自分のアセンブリのサテライトを
  共有フレームワークのフォルダーでしか探さないので、FrameworkOnCore の System.Web(`FrameworkSatellites`)がアプリの
  `bin\<言語>` を探します。
- 使う言語は Studio の「アプリの設定」の「言語パック」(`foc-choices.json` の `settings.languages`)で選びます。.NET Framework の言語パックの 23 言語と
  英語(原文)から複数選べます(`en,ja,de`)。既定は、英語と、元のサーバーに .NET Framework の言語パックが入っていた言語
  (`capture-culture.ps1` が LanguagePacks に記録。無ければ変換したマシンの)。英語だけを選べば、UI カルチャにかかわらず英語です。
  選ばれていない言語の訳は bin から除きます。訳がまだ無い言語を選んだときは、変換レポートに出て、その言語では英語になります。
- 訳の出どころは部品 `language-packs` で選びます: `translations`(既定。この翻訳)、`framework`(Microsoft の言語パック。Linux に配置しないときだけ)。
- 訳の無い文言は英語になります。

## ファイルの形

`.resx` の各項目は、名前がそのリソースの名前、`value` が訳、`comment` が訳した時の英語の原文です。

```xml
<data name="Calendar_PreviousMonthTitle" xml:space="preserve">
  <value>前の月を表示</value>
  <comment>Go to the previous month</comment>
</data>
```

- `{0}`・`{1}` などのプレースホルダーは、原文と同じものをすべて残してください(順番は変えてかまいません)。
- 英語の原文の場所は `sources.json` にあります(例: System.Web は `FrameworkOnCore.Runtime/src/WebFormsForCore.Web/System.Web.resx`)。

## 確認

```powershell
.\translations\check.ps1
```

- エラー(終了コード 1): リソースに無い名前、プレースホルダーの違い、空の訳、.NET Framework の文言より表示幅が広い訳(下の「長さ」)。
- 警告: `comment` の英語が今の原文と違う(原文が変わったので訳を見直す。見直したら `comment` を今の原文にする)。
- 単体テスト(`tests/FrameworkOnCore.Tests` の TranslationsTests)も同じことを確かめます。

## 訳を足す・直す

1. GitHub で Issue を立てるか、このフォルダーの `.resx` を直した Pull Request を送ってください。
2. 新しい言語は `<言語>\<アセンブリ>\<リソース>.<言語>.resx`(例: `zh-Hans\System.Web\System.Web.zh-Hans.resx`)を作ります。
   訳していない文言は英語になるので、一部のアセンブリやリソースだけでもかまいません。言語の名前は .NET Framework の言語パックの
   カルチャ名(`ja`、`de`、`zh-Hans` など)にしてください。
3. `src/FrameworkOnCore.Analysis/catalog/components.json` の設定 `languages` で、その言語に `"translated": true` を付けてください(単体テストが、訳のフォルダーと一致するか確かめます)。
4. `check.ps1` が通ることを確かめてください。

## 長さ

画面のレイアウトが崩れないよう、訳は .NET Framework がその言語で表示していた文言より**表示幅が広くなってはいけません**(全角 2・半角 1 で数えます)。
`check.ps1` は、このマシンに .NET Framework のその言語の言語パックがあるとき、その文言の表示幅だけを読んで比べます(文言そのものは使いません)。
.NET Framework に無い文言(System.Web.Optimization や、FrameworkOnCore で増えた文言)や空の文言は、英語の表示幅(12 未満なら + 4)までです。
言語パックが無いマシンでは幅は検査されないので、日本語の Windows などで確かめてください。

## 訳の方針(日本語)

- 画面の文言は、短く、です・ます調(ボタンやラベルは体言止め)。必須の入力のメッセージは「〜は必須です。」。
- 用語: Web パーツ、ゾーン、カタログ、エディター、プロバイダー、コンシューマー、電子メール アドレス、ユーザー名、パスワード、秘密の質問。
- 英語の固有の名前(`Debug="true"`、`<customErrors>`、`IUSR_MACHINENAME` など)は訳さない。
