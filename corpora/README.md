# コーパス

変換の適用範囲は、実在する WebForms アプリを変換したときの**残差の件数**で測ります。
その測定対象をここで取得・変換・比較します。

コーパス本体はサードパーティのソースで、それぞれ独自のライセンスを持ちます。
リポジトリには含めず、`fetch.ps1` が既知のバージョンを `corpora/work/` に取得します
(`corpora/work/` と `corpora/out/` は `.gitignore` 済み)。

## 使い方

```powershell
.\corpora\fetch.ps1          # コーパスを取得(初回のみ、約 140MB)
.\corpora\convert-all.ps1    # 全件変換してベースラインと比較
```

`convert-all.ps1` はベースライン(`expected.json`)と一致すれば exit 0、
差異があれば exit 1 を返します。**変換器を触ったら必ずこれを回してください。**

```powershell
.\corpora\convert-all.ps1 -Only be,yaf     # 対象を絞る
.\corpora\convert-all.ps1 -SkipBuild       # 変換器のビルドを省く
.\corpora\convert-all.ps1 -UpdateBaseline  # 実測値を expected.json に書き戻す
```

`-UpdateBaseline` は**変化の理由を説明できるときだけ**使ってください。
数字が動いた理由を確認せずに更新すると、ベースラインは回帰検知の役に立たなくなります。

## 対象

| 名前 | コーパス | 取得元 |
|---|---|---|
| `be` | BlogEngine.NET 3.3.8.0 | `rxtur/BlogEngine.NET` @ `v3.3.8.0` |
| `mojo` | mojoPortal 3.1.6 | `i7MEDIA/mojoportal` @ `v3.1.6` |
| `yaf` | YAF.NET 3.2.15 | `YAFNET/YAFNET` @ `v3.2.15` |
| `dnn` | DNN Platform 9.13.10 | `dnnsoftware/Dnn.Platform` @ `v9.13.10` |
| `n2` | n2cms | `n2cms/n2cms` @ `master` |
| `wt` | WingtipToys | `corn-mendoza/wingtiptoys` @ `master` |

`n2` は**ホールドアウト**として後から追加したものです。他の 5 本は変換器を育てる過程で
使ってきたため、それらへの適合しすぎを検出する対照が必要でした。追加時点の初回変換で、
他の 5 本が一度も踏まなかった穴(ジェネリックなページ基底クラス)を実際に露出させ、
生成 Razor のエラーが 1 件から 108 件に跳ね上がっています。

**変換器を変更したら、`n2` の数字を特に見てください。** ここが動くときは、他 5 本に
合わせ込んだ変更である可能性があります。

### nopCommerce 1.90 について

6 つめのコーパスとして以前計測していましたが(残差 264 / 変換可能 32)、
**現在は自動取得できません。** nopCommerce 1.x は CodePlex 時代のリリースで、
GitHub の `nopSolutions/nopCommerce` に 1.x のタグが無く、ミラーも見つかりませんでした。

手元にアーカイブがある場合は `corpora/work/nopcommerce-1.90` に展開すれば計測できます
(`convert-all.ps1` の対象表への追加が必要)。

なお、以前リポジトリにあった `samples/nopCommerce3.8` は **3.8 = ASP.NET MVC** で、
このコーパス(1.90 = WebForms)とは別物です。変換対象にはなりません。

## 変換オプションは固定してある

**残差の前後比較は変換オプションを揃えて初めて成立します。** `--input` だけ合わせても
比較になりません。オプションを 1 つ落とすと数字は桁で動きます。

そのためオプションは `convert-all.ps1` の表に固定してあり、コマンドラインからは変えられません。
実際に踏んだ罠:

| コーパス | 必要なオプション | 落とすとどうなるか |
|---|---|---|
| `be` | なし | — |
| `mojo` | `--include mojoPortal.Data.MSSQL`、`--control-map` | プロバイダ未指定ではデータ層が移植されない / `<mp:mojoGridView>` が未対応コントロール扱いになる |
| `yaf` | `--project YAF-SqlServer.csproj`、`--web-config recommended.web.config` | `.csproj` が 4 つあり自動導出されない / `tagPrefix` が読めず `<YAF:LocalizedLabel>` などが UnmappedControl に爆発する |
| `dnn` | なし | — |
| `n2` | `--project N2.Templates.csproj`、`--expression-map` | `.csproj` が 7 つある / 独自の式ビルダーが全て残差になる |
| `wt` | なし(入力ルートが 1 階層深い) | — |

### 式ビルダーの対応表(`--expression-map`)

n2 の「変換可能」残差 59 件のうち **23 件は独自の式ビルダー**でした
(`<%$ CurrentItem: Title %>` など)。AI 残差層のタスクを生成させると 17 件全部が
これで、**AI に投げる前に決定的層で片が付く**種類のものでした。

対応表は推測ではありません。各ビルダーが**変換先の C# 式を自分で宣言しています**。

```csharp
// src/Framework/N2/Web/Compilation/CurrentItemExpressionBuilder.cs
get { return @"N2.Web.Compilation.CurrentItemExpressionBuilder.GetCurrentItemValue(""{0}"")"; }
```

`corpora/expression-maps/n2.json` はこの文字列を `{0}` → `{value}` に置き換えただけです。
呼び先の静的メソッドは N2 本体と一緒に移植されるので、新しい実装は要りません。

**`Code` プレフィックスは意図的に外してあります。** `CodeExpressionBuilder` は式を
そのまま埋め込む(`CodeSnippetExpression`)ので対応表は `{value}` になりますが、
値が文字列リテラルを含むと(`<%$ Code: "AutoZone2" %>`)エミッタが属性値として
エスケープし、`@(\"AutoZone2\")` という**不正な Razor** になります。実際に n2 の
UITests TemplatePage.aspx で構文解析が止まり、ビルドエラーが 2 件増えました。
対応するにはエミッタ側で「生コード」と「文字列値」のテンプレートを区別する必要があります。
それまでは残差のままにしてあります。

**参照ライブラリの `--include` は不要になりました。** 変換器が入力アプリの `.csproj` から
`ProjectReference` を推移的に辿って自動で移植対象にします。ここに残っているのは
機械が決められない 2 種類だけです。

- `--project` — `.csproj` が複数あるとき。データベースごとにビルド構成を分ける実装が該当し、
  推測せず何も導出しません
- `--include` — 同じ型を宣言する排他プロジェクト群(`mojoPortal.Data.*`)から 1 つ選ぶとき。
  どれも自動採用しません

YAF はサイトルートに `Web.config` が無く、配布時に `recommended.web.config` を
リネームする前提になっています。これを渡さないと `tagPrefix` が読めません。

### 判定の勘所

**「移植 .cs」の数が動いていたら、変換器ではなくオプションかコーパスの取得内容が違っています。**
その状態で残差を比較しても意味がありません。`convert-all.ps1` はこれを最初に見ます。

## ベースライン(`expected.json`)

```
コーパス  移植 .cs  総残差  変換可能  ビルドエラー
be            253      83         6             0
n2           1661     232        36            95
mojo          731     127        20           211
yaf          2722      73         5            83
dnn          1944     367        12           661
wt             13      39         3             1
合計                  921        82          1051
```

### 残差を原因で数える

分類前は 1,095 件で、内訳はこうでした。

```
973  手動移行     コードビハインド 512 / 未対応コントロール 426 / 構成 35
 82  変換可能     未対応の属性 36 / インラインコード 32 / データバインド式 9 / その他 5
 40  情報通知
```

**「未対応コントロール 426 件」は 1 つの問題ではありませんでした。** 種別名は
「マッピング表への追加が必要」ですが、実際には 3 種類が混ざっていました。

| | 件数 | 実態 |
|---|---|---|
| `portal:mojoButton` | **128** | **マッピング不要だった。**変換元は移植済み、`LegacyWebControl` を継承、`tagPrefix="portal"` も登録済み。**解決器の欠陥**(下記、修正済み) |
| `Register` の `Src` 先が無い | 97 | `~/DesktopModules/DDRMenu/Actions.ascx` などが入力ツリーに存在しない。**ファイル解決の問題**でマッピングとは無関係 |
| 実際に未対応の他社コントロール | ~200 | `dnn:DnnComboBox` 16、`n2:ItemDataSource` 15、`YAF:*` など。ここだけが本当にマッピング表の話 |

**426 件のうち 225 件は「マッピング表に足せ」という案内が誤り**でした。

### LegacyRenderHost の対象基底クラス

`ResolveLegacyControl` は、移植されたコントロールの**元の基底チェーンの根**が
`LegacyRenderableRoots` にあるときだけ `LegacyRenderHost` に解決します。この集合は
必要になるたびに 1 つずつ足されてきたもので、**`Button` が入っていませんでした。**

そのため `mojoButton : Button` は「未対応コントロール」として 128 回報告されていました。
全コーパスの未対応コントロールの 30% が、これ 1 つです。

`Button` / `TextBox` / `CheckBox` / `DropDownList` / `CompositeControl` などを追加し、
**総残差 1,095 → 921(−174)**。ビルドエラーは 1,100 のまま動いていません。

これは「報告する」から「**アプリ自身の描画コードで描画する**」への挙動変更です。
安全なのは、`LegacyRenderHost` が呼ぶのが移植された元クラスの `RenderContents` 等
そのものだからで、互換層が挙動を推測するわけではありません。パリティ 30/30 で確認済みです。

### コードビハインド 512 件 — 半分は WebForms ではない

同じ手法で割ると、321 件が「.NET Framework 専用の名前空間」の 1 形でした。その中身:

```
140  System.Web.Mvc
 79  System.Web.Http           ← Web API
 22  System.Web.Http.*(3 種)
 31  Microsoft.AspNet.Identity
 22  System.Web.Routing
  4  System.Web.Optimization
```

**MVC + Web API + Identity + バンドルで 232 件。** DNN は WebForms と MVC と Web API が
1 つのアプリに同居しており、**この変換器の対象は WebForms だけ**です。

これらに「手動移行が必要」と書くのは誤解を招きます。**変換で失われたものは何もなく、
最初から対象外**だからです。読み手には次の 2 つを区別できる必要があります。

- 変換器が落としたので作り直す必要があるもの
- そもそも別フレームワークで、この道具の範囲外のもの

メッセージを分けました。

```
ASP.NET MVC(System.Web.Mvc)のコードです。この変換器は WebForms のみを対象とするため
移植していません。変換で失われたものはなく、対応する ASP.NET Core の仕組みへ別途移行してください。
```

### ビルドエラー側には、この切り分けはほぼ不要でした

残差で 232 件が対象外だったので、ビルドエラー 1,115 件にも同じ比率で混ざっていると
考えて測りましたが、**対象外フレームワークに言及するエラーは 15 件だけ**でした。

理由は考えれば当然で、**MVC を import しているファイルは移植前に除外される**ため、
そもそもビルドに参加しません。残差になった 232 件がまさにそれです。ビルドエラーに
出てくるのは「移植されたファイル」だけなので、対象外コードは原理的にほとんど現れません。

`AuthorizeAttribute.AuthorizeCore` のような MVC 由来のものは確かに存在しますが、
それは `System.Web.Mvc` を直接 import していなかったために除外を免れた少数です。

**サンプル 2 行から「多く混ざっている」と書いたのは誤りでした。** 実測は 15/1115 です。

### ビルドエラー 1,115 件の実際の構成

```
401  CS0246  型が見つからない
295  CS0115  オーバーライド対象が無い
230  CS0759  partial メソッド未定義   ← --analyzer で 0 になる
 60  CS0234
 36  CS0104  → 内訳は下記(28 件が互換層、44 件は BCL との衝突)
 15  CS0535
```

CS0115 のオーバーライド先を数えると、`RenderEditMode` / `RenderViewMode` /
`StringValue` / `AllowableFiles` / `ItemNodeName`(DNN の `EditControl` 系)と、
`OnInit` / `CreateChildControls` / `LoadViewState` / `SaveViewState`(WebForms
ライフサイクル)に集中していました。**いずれも除外型スタブが基底**でした。

### スタブは protected メンバを落としていた

原因は 1 行です。スタブ生成器はメンバを `public` と `internal` だけ通していました。

```csharp
protected abstract string StringValue { get; set; }      // 元
protected virtual void RenderViewMode(HtmlTextWriter w)  // 元
```

**サブクラスが override するメンバは、たいてい protected です。** 落とせば、その型を
継承している移植済みクラスが全部 CS0115 になります。DNN の `EditControl` がこれで、
`RenderEditMode` / `RenderViewMode` / `StringValue` / `AllowableFiles` を失っていました。

protected を通し、**アクセス修飾子は元のまま**にしました(public に上げると型の公開面が
変わるため。ここに置く理由は override させることだけです)。**DNN 710 → 661。**

スタブ生成器はこれで 3 回目です — 基底クラスの消失(CS0115)、修飾子の消失(CS0506)、
メンバの消失(CS0115)。いずれも**エラーはサブクラス側にしか出ません**。
除外型スタブが絡む override エラーを見たら、まず生成器を疑ってください。

「変換可能 36 件の未対応属性」も同様に固まっています。

```
18  DataSourceID 13 + DataSource 4 + DataMember 1   宣言的データバインド(1 機能)
 7  resourcekey                                      暗黙ローカライズ
11  その他(単発)
```

**追うべきは「変換可能」と「ビルドエラー」です。** 総残差の大半は `ManualMigration`
(設計判断・外部依存)で、コーパスが持ち込む依存の量を測っているにすぎません。
分類の定義は `ResidualDisposition`(`src/WebForm2Blazor.Converter/ConversionReport.cs`)と
[../HANDOVER.md](../HANDOVER.md) の 2.3 を参照してください。

**残差とビルドの通りやすさは別の指標です。** 除外型スタブの変更で BlogEngine が
0 → 17 エラーに退行したとき、残差の数値は 3 つとも一切動きませんでした。
そのため `convert-all.ps1` は変換のたびに `--verify-build` を回し、ビルドエラー数も
ベースラインとして固定します(`-SkipVerifyBuild` で省略可。所要時間は数分増えます)。

「変換可能」やビルドエラーが減るのは改善なので成功扱いにしますが、
**増加した場合や、総残差・移植 .cs が動いた場合は理由を確認するまで失敗扱い**にします。

なお構文エラーがあるとコンパイラは意味解析を行わないため、**ビルドエラー数は総数ではなく
下限**になります(YAF で構文エラー 1 個が 1,628 件を隠していた実例があります)。
その状態を検出したら `convert-all.ps1` は警告して失敗扱いにします。

**コンパイラに届く前にビルドが落ちた場合も同じことが起きます。** 実例として、`App_Data` を
`<Content Include>` で追加したことが SDK 既定のグロブ(`**/*.config`)と衝突し、n2cms が
`NETSDK1022` 1 件でビルド開始前に停止していました。件数は 109 → 1 に「改善」したように
見え、**その 1 という数字をベースラインとして固定してしまっていました**(実際にはコンパイル
自体が一度も走っていません)。この状態では AI 残差層のゲートも機能せず、意図的に構文を
壊した回答を「エラー増加なし」として受理します。

検出は「構文エラーコードの一覧」ではなく **CS 診断が 1 件も出ていないのにエラーがある**
という条件で行います。`NETSDK*` / `MSB*` / `RZ*` を列挙せずに閉じられるためです。

## global using を使うアプリ

C# 10 以降のアプリは `GlobalUsings.cs` に `global using` を並べ、個々のファイルには
using を 1 行も書きません。変換器はそこから `System.Web.*` の import を落として互換層の
import を足しますが、**足す側が平の `using` だった**ため、その 1 ファイルにしか効かず、
プロジェクト中のファイルが型を見失っていました。

YAF.NET がこれで、`HtmlTextWriter` 54 件・`HttpContext` 22 件が「互換層に存在するのに
CS0246」という状態でした。**注入する using を元ファイルの global に合わせた結果、
179 → 114 件**になっています。

副作用として mojoPortal が 1 件増えました(`Image` が `System.Drawing.Image` と衝突)。
互換層が project 全体から見えるようになったことで、下記の平坦化問題が 1 箇所表面化した
ものです。差引 64 件の改善なので受け入れています。

## リポジトリ同梱 DLL への参照

WebForms 期のアプリは依存ライブラリを `_libs\` などにチェックインし、`<Reference>` +
`<HintPath>` で参照します。**NuGet の識別子が無いので引き継ぎようがなく**、変換器は
黙って落としていました。mojoPortal の Lucene.Net がこれで、CS0246 が 115 件出ていました。

**変換器は代替パッケージを選びません。** Lucene.Net 3.0.3 → 4.8 のようにメジャーが
変わって API が別物になっているものがあり、機械的に決められないためです。

代わりに、**答えを書き込める形で出力します。** 変換のたびに出力先へ
`package-map.template.json` を書き、そこに未決の依存が並びます(mojo は 61 件)。

```json
[
  { "_readme": "..." },
  { "assembly": "Lucene.Net", "package": "", "version": "" }
]
```

`package` と `version` を埋めて `--package-map <そのファイル>` で再変換すると
`PackageReference` が入ります。**引き継がないと決めたものは `package` を空のまま**に
すると残差から消えます — 「判断した」と「判断していない」を区別するためです。

テンプレートはそのまま `--package-map` に渡せる有効な JSON です(説明は
`assembly` を持たないエントリに入れてあり、読み込み側が無視します)。

一方 **HintPath の無い `<Reference Include="..." />` は GAC アセンブリ**で、こちらは
同一 API の NuGet パッケージがあります。`System.ComponentModel.Composition`(MEF)を
名前空間対応表に追加し、**DNN が 941 → 901** になりました。

## あいまい参照 CS0104(未解決)— まず内訳を見てください

**「CS0104 = 互換層の平坦化が原因」は誤りです。** 実測すると 72 件中 28 件しか
互換層は関係していません。名前空間分割に着手する前にこれを確認してください。

```
28  互換層 vs アプリ      TreeNode 20 (n2) / ListItem 6 (yaf) / Image 2 (mojo)
44  アプリ同梱コード vs BCL  Activity 12 / Directory 8 / *Converter 10 /
                           OrderedDictionary 4 / Attribute 2 / BBCode 2 ほか
```

後者は互換層と無関係で、**移行先の .NET が新しいことによる衝突**です。
`System.Collections.Generic.OrderedDictionary` は .NET 9 で新設された型なので
.NET Framework 時代には衝突しようがなく、`System.Diagnostics.Activity` も同様です。
アプリが同梱した Lucene.Net や ServiceStack の型が、後から増えた BCL の型と
名前でぶつかっています。**互換層をどう分割してもこの 44 件は動きません。**

### 互換層側の 28 件

互換層は `System.Web` 全体を `WebForm2Blazor.Components` 1 つに平坦化します。その結果、
**`System.Web` しか import していなかったファイルにも `System.Web.UI.WebControls` 相当の型が
見えるようになり**、アプリが同名の型を持っていると `CS0104` になります。
n2cms の `N2.Edit.TreeNode` と互換層の `TreeNode` がその 20 件です。

**「プロジェクト型を優先する別名を出す」方向で 2 回試し、いずれも撤回しました。**

1. 「プロジェクト名前空間を import しているならプロジェクト型を意図している」
   → `Page` のように元は System.Web 側から来ていた名前まで奪い、**YAF が 187 → 739 に悪化**
2. 「元ファイルが `System.Web.UI*` を import していない場合に限る」
   → n2 は 3 件改善したが YAF が 5 件退行し、差引で悪化

どちらも「その名前が元は互換層側から来ていたのか」を確実に判定できないことが原因です。
根治するには**互換層の名前空間を元の `System.Web.*` の構造に合わせて分割**し、
変換器が元ファイルの import に対応する名前空間だけを補う必要があります。

**ただし、上の内訳を踏まえると費用対効果は低いです。** 互換層の全ファイルに影響する
変更で、上限は 28 件です。同じ労力なら他に効く場所があります。着手するなら
「28 件のために互換層を作り直す」と分かった上で判断してください。

## 既知の注意点

- 出力は毎回作り直されます。前回の生成物が残っているとレポートが混ざり、
  「直したはずの残差がまだ出る」ように見えます(実際に一度これで誤読しました)。
- (解決済み) `wt` の `NVPAPICaller` 未解決は、除外判定がコメント文中の "PayPal." に
  反応していたためでした。変換元にはファイルが存在します。
- これらのスクリプトは PowerShell 5.1 互換です。日本語を含むため
  **BOM 付き UTF-8** で保存してください(BOM 無しだと PS 5.1 は ANSI として読み、
  文字列リテラルが壊れます)。
- **`-SkipBuild` を使うときは互換層と変換器を自分でビルドしてください。** 互換層の
  コンパイルが失敗すると全コーパスがその失敗分だけを報告し(実測: 全 6 本が揃って
  「8 件」)、スクリプトはそれを**改善として通します**。同じ理由で、互換層が壊れている間は
  変換器のビルドも失敗するので、`bin\alt` が古いまま計測される事故も起きます
  (実際に 836→830 と読み違え、再ビルド後の正しい値は 836→779 でした)。
  引数なしの `convert-all.ps1` は両方をビルドするのでこの穴はありません。

## 除外型スタブの基底クラス

移植対象外になった型は `ExcludedTypeStubs.g.cs` にスタブとして出ます。このスタブが
**基底クラスを失うと、そのスタブを継承しているクラスの override が全部落ちます。**

DNN の `DnnDropDownList : Panel` が実例でした。`Panel` は互換層では Blazor
コンポーネントなので、平のスタブクラスは継承できません。結果スタブは基底無しになり、
`DnnFileDropDownList` などが `OnInit` / `OnPreRender` / `CreateChildControls` の
override で CS0115 になっていました。**エラーはサブクラス側に出るので、原因が
スタブにあることは表示からは分かりません。**

WebForms コントロールが基底の場合は `LegacyWebControl`(平のクラスとしての
コントロール基底)に解決するようにし、**DNN が 836 → 779** になりました。

### スタブのメンバは `virtual` を保つ

同じ理由で、スタブは**元が `virtual` / `abstract` / `override` だったメンバを
`public void Install()` として出していました。** 継承側は移植されているので、
その override が全部 CS0506(「基底が virtual でない」)になります。DNN の
`FileInstaller` / `PermissionsGrid` / `AuthorizeAttributeBase` がこれでした。

元の修飾子を写すだけにしてあります(元が overridable でなければ付けません)。
**DNN 779 → 710、CS0506 は 71 → 0。**

### 残っている CS0115(241 件)

こちらは「基底にメンバ自体が無い」ケースで、2 つに分かれます。

- 互換層の不足(`Control.EnableViewState` など)
- **ASP.NET MVC の型**(`AuthorizeAttribute.AuthorizeCore(HttpContextBase)` など)。
  DNN は WebForms と MVC が同居しているアプリで、MVC 部分はこの変換器の対象外です

前者は互換層に足せますが、後者は「範囲外」であって欠陥ではありません。
**着手する前に、この 2 つを分けて数えてください。**

## ソースジェネレータ(`--analyzer`)

DNN Platform はビルド時に Roslyn ソースジェネレータを走らせます。`[DnnDeprecated]` から
partial メソッドの**定義宣言**を生成するもので、その宣言は**変換元のソースに存在しません**。
変換器はソースを読み書きするだけなのでそれを再現できず、依存するコードがビルドできません
(CS0759 が 230 件)。

ジェネレータを変換器が自前で実行するのではなく、**生成プロジェクトに `<Analyzer>` として
組み込みます**。ビルド時に MSBuild が元と同じようにジェネレータを走らせるため、参照解決も
意味解析も自前で用意する必要がありません。

```powershell
.\corpora\convert-all.ps1 -Only dnn -WithAnalyzers
```

`-WithAnalyzers` は必要ならジェネレータを先にビルドしてから `--analyzer` で渡します。

**実測: DNN Platform のビルドエラー 836 → 606、CS0759 は 230 → 0**(減少幅が CS0759 の
件数と一致します)。

**ベースラインには入れません。** ジェネレータのビルドには**そのプロジェクトが `global.json`
で固定した SDK** が要ります(DNN は 9.0.202 / `rollForward: latestMinor`)。ベースラインが
「どの SDK がインストールされているか」で動くようになると、回帰検知の役に立ちません。
このマシンで通るのは 9.0.317 が入っているからで、それは環境の事情です。

つまり `-WithAnalyzers` は**計測用であって比較用ではありません。** ジェネレータをビルド
できなかった場合、スクリプトは警告を出して `--analyzer` 無しで続行します(黙って別物の
数字を出さないため)。

## BlogEngine の稼働状況

```
/          200
/archive   200
/search    200
/contact   200
/post      500   (下記「Init と @ref の順序」)
```

**変換出力がロールデータを実際に読んで動いています。** ロールは `App_Data/roles.xml` から
`Administrators / Editors / Anonymous` が読み込まれ、移植された `XmlRoleProvider` が
そのまま供給しています。

ここに至るまでに解いた 5 段階(いずれも汎用の仕組みとして実装):

| 症状 | 原因 |
|---|---|
| `GetSection` が null | カスタム構成セクションが引き継がれていなかった |
| `Could not resolve type` | 型のアセンブリ修飾が旧アセンブリのままだった |
| `Unable to load default provider` | `ProvidersHelper.InstantiateProviders` が空実装 |
| `DirectoryNotFound` | `App_Data` 未コピー / `AppDomainAppPath` が bin を指していた |
| **スタックオーバーフロー** | **`Roles` がロールプロバイダに委譲していなかった** |

最後の 1 つは `Right.RefreshAllRights` ↔ `SaveRights` の無限再帰として現れました。
`GetAllRoles()` が空を返すと BlogEngine は「権限テーブル未設定」と判断して既定値を
書き込み、読み直し、また空を見る、を繰り返します。

**fail closed の方針(HANDOVER 2.4)は維持しています。** ロールストアが**無い**ときは
従来どおり空を返し、`IsUserInRole` も false です。変わったのは、**アプリ自身のロール
プロバイダとそのデータが変換出力に揃っているときに、それを使うようになった**点だけで、
ロールを捏造してはいません。

`/contact` は 3 つの修正で通るようになりました。いずれも汎用の欠陥です。

| 症状 | 原因 | 修正 |
|---|---|---|
| `NullReferenceException` | `<label for="<%= txtName.ClientID %>">` — `@ref` は初回描画後にしか代入されない | 同一ファイルで宣言された ID の `X.ClientID` を `ClientIdOf("X")` に書き換え |
| `ChildContent が無い` | `<asp:Label>本文</asp:Label>` のタグ内容 | `Label` に `ChildContent` を追加(`Text` 優先は WebForms と同じ) |
| `parameter ... is not public` | getter のみのプロパティに `[Parameter]` を付与していた | public セッターがあるものだけに限定 |

3 つ目は**実行時**にしか出ません。ビルドは通り、そのコンポーネントが初めて描画された
瞬間に落ちます。

### Init と @ref の順序(未解決)

`/post` が残っています。**個別ページの残差ではなく、ライフサイクルの構造的な差です。**

WebForms はコントロールツリーを構築してから `OnInit` を呼ぶため、`OnInit` の中で宣言済み
コントロールに触れるのは普通のコードです(`ucCommentList.Visible = ...`)。Blazor では
それらは `@ref` フィールドで、**初回描画後にしか代入されません**。

`OnInit` を `OnAfterRender(firstRender)` に遅らせる修正を試し、**撤回しました。**
`Page_Load` が既にそうなっているので一貫して見えますが、`OnInit` は描画に必要なデータを
作る側でもあります(BlogEngine の Post ページは `OnInit` でページ全体がバインドする
`Post` を代入します)。遅らせると `ucCommentList` の null は消えますが、今度は `Post` が
null になり、NullReferenceException が別の場所に移動しただけでした。

2 つの制約 —「描画前に走る必要がある」と「描画後にしか存在しないものを使う」— は
Blazor のライフサイクルでは同時に満たせません。根治するには `@ref` フィールドを、
生成後に実体へ委譲するプロキシにして Init 中の設定を保留・再生する設計が要ります。
影響範囲が大きいため、着手するなら独立した作業として計画してください。
