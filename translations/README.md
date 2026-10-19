# FrameworkOnCore の翻訳

ASP.NET(System.Web)が自分で出す文言の、FrameworkOnCore 独自の翻訳です。Calendar の見出し、Login・CreateUserWizard などのラベルとメッセージ、
Wizard・GridView のボタン、Menu・TreeView の補助テキスト、Web パーツの動作名、エラーページの見出しと説明がその言語になります。

## なぜ独自に訳すのか

.NET Framework では、言語パック(日本語の Windows には普通入っている)があるサーバーで、UI カルチャがその言語のとき、これらの文言がその言語になっていました。
.NET には言語パックが無く、FrameworkOnCore の System.Web の文言は英語(referencesource)だけです。

Microsoft の言語パックは、使用許諾(MICROSOFT SOFTWARE SUPPLEMENTAL LICENSE TERMS – .NET FRAMEWORK AND ASSOCIATED LANGUAGE PACKS FOR MICROSOFT WINDOWS
OPERATING SYSTEM)の 1.c で「Windows 以外のプラットフォームで動かすための配布」を禁じています。Linux で動かす FrameworkOnCore には使えないので、
英語の原文から独自に訳したものをここで管理します。

**訳すときに、Microsoft の言語パックの訳文(Windows の .NET Framework が出す日本語)を写したり参考にしたりしないでください。** 英語の原文から訳してください。
そのため、文言が元のサーバー(.NET Framework)の表示と同じとは限りません。

## 使われ方

- `<言語>\System.Web.<言語>.resx` は FrameworkOnCore.Web パッケージのサテライト(`lib/net10.0/<言語>/System.Web.resources.dll`)になります
  (FrameworkOnCore.Runtime の LICENSE と同じ MIT)。
- 使う言語は Studio の「アプリの設定」の「言語パック」(`foc-choices.json` の `settings.languages`)で選びます。.NET Framework の言語パックの 23 言語と
  英語(原文)から複数選べます(`en,ja,de`)。既定は、英語と、元のサーバーに .NET Framework の言語パックが入っていた言語
  (`capture-culture.ps1` が LanguagePacks に記録。無ければ変換したマシンの)。英語だけを選べば、UI カルチャにかかわらず英語です。
  選ばれていない言語の訳は bin から除きます。訳がまだ無い言語を選んだときは、変換レポートに出て、その言語では英語になります。
- 訳の出どころは部品 `language-packs` で選びます: `translations`(既定。この翻訳)、`framework`(Microsoft の言語パック。Linux に配置しないときだけ)。
- 訳の無い文言は英語になります。

## ファイルの形

`.resx` の各項目は、名前が System.Web のリソースの名前、`value` が訳、`comment` が訳した時の英語の原文です。

```xml
<data name="Calendar_PreviousMonthTitle" xml:space="preserve">
  <value>前の月を表示</value>
  <comment>Go to the previous month</comment>
</data>
```

- `{0}`・`{1}` などのプレースホルダーは、原文と同じものをすべて残してください(順番は変えてかまいません)。
- 英語の原文は `FrameworkOnCore.Runtime/src/WebFormsForCore.Web/System.Web.resx` にあります。

## 確認

```powershell
.\translations\check.ps1
```

- エラー(終了コード 1): System.Web に無い名前、プレースホルダーの違い、空の訳。
- 警告: `comment` の英語が今の原文と違う(原文が変わったので訳を見直す。見直したら `comment` を今の原文にする)。
- 単体テスト(`tests/FrameworkOnCore.Tests` の TranslationsTests)も同じことを確かめます。

## 訳を足す・直す

1. GitHub で Issue を立てるか、このフォルダーの `.resx` を直した Pull Request を送ってください。
2. 新しい言語は `<言語>\System.Web.<言語>.resx`(例: `zh-Hans\System.Web.zh-Hans.resx`)を作ります。言語の名前は .NET Framework の言語パックの
   カルチャ名(`ja`、`de`、`zh-Hans` など)にしてください。
3. `src/FrameworkOnCore.Analysis/catalog/components.json` の設定 `languages` で、その言語に `"translated": true` を付けてください(単体テストが、訳のフォルダーと一致するか確かめます)。
4. `check.ps1` が通ることを確かめてください。

## 訳の方針(日本語)

- 画面の文言は、短く、です・ます調(ボタンやラベルは体言止め)。
- 用語: Web パーツ、ゾーン、カタログ、エディター、プロバイダー、コンシューマー、電子メール アドレス、ユーザー名、パスワード、秘密の質問。
- 英語の固有の名前(`Debug="true"`、`<customErrors>`、`IUSR_MACHINENAME` など)は訳さない。
