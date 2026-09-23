# WebForm2Blazor 引き継ぎ

このドキュメントは**開発を引き継ぐ人**向けです。使い方・変換対応表・コマンドは
[README.md](README.md) にあります。ここには README に書いていないもの
——「なぜそう作ったか」「いまどこまで動くか」「何が壊れているか」——を書きます。

最終更新: 2026-08-14 時点の実測値。

---

## 1. 目的

.NET Framework の **ASP.NET WebForms アプリを Blazor Server へ変換する汎用ツール**です。
特定の 1 アプリを移植するスクリプトではなく、任意の WebForms アプリを入力に取れることを狙っています。

「汎用」を測るために、実在する 6 つの OSS WebForms アプリをコーパスとして変換し、
**変換できなかった箇所(残差)の件数**を指標にしています。残差が減る = 適用範囲が広がる。

意図的に**やらない**こと:

- 完全自動移行の約束。WebForms の設計判断(ポストバック、動的コントロール生成、
  HttpModule)には Blazor に自然な対応物がないものがあり、それらは人間が決めるべき残差として報告します。
- 「それらしく動く」出力。**動かないコードを黙って出すより、見える残差を出す**方を常に選びます。

---

## 2. 設計上の判断

引き継ぐ上でこれだけは踏襲してほしい、という順に並べます。

### 2.1 正解は「旧アプリが生成する HTML」

変換が正しいかどうかは、仕様書でも記憶でもなく、
**IIS Express で実際に動いている変換前の WebForms アプリの描画**で決めます。

`tools/WebForm2Blazor.ParityTest` が旧アプリを Playwright で操作して DOM スナップショットを
ゴールデンマスターとして記録し(`record`)、変換後の Blazor アプリに同じ操作をして
突き合わせます(`verify`)。現在 **30 スナップショットが完全一致**しています。

この規律から派生する重要な帰結:

- **既定値は実測から採る。** 「WebForms の `CellPadding` の既定は確か -1」ではなく、
  4.8 で実際に描かせて確認する。`samples/DefaultsProbe` はそのための実験台で、
  1 ページに 60 以上のコントロールを並べて既定レンダリングを固定しています。
- **書かれない既定値は残差にも出ない。** マークアップに `GridLines` が書かれていなければ
  「未対応属性」の残差は立ちません。属性のパリティ層(DefaultsProbe + PROPERTY-COVERAGE.md)だけが
  この穴を塞ぎます。残差 0 件は「完璧」ではなく「書かれた構文は全部読めた」の意味です。

### 2.2 決定的変換が主、AI は残差だけ

変換は 4 層構成です。

| 層 | 役割 | 実体 |
|---|---|---|
| 1. 棚卸し | 入力プロジェクトの走査、タグプレフィックス解決、基底クラス登録 | `Project/WebFormsProject.cs`, `BaseClassRegistry` |
| 2. 決定的変換 | ASPX パース → Razor 生成、Roslyn によるコードビハインド書き換え | `Parsing/`, `Emit/`, `Convert/` |
| 3. AI 残差層 | 決定的層が扱えなかった構文を LLM に投げる | `AI-TASKS.json` を出力(実行は外部) |
| 4. 検証 | bUnit / ビルド検証 / パリティ | `tests/`, `tools/` |

**AI は主役ではありません。** LLM に投げるのは「決定的層の穴」だけで、それが
`ResidualDisposition.Convertible` の定義です。AI 層で繰り返し出るパターンを見つけたら、
`ControlMappings` に「昇格」させて決定的層の守備範囲を広げる、という運用を想定しています。

### 2.3 残差を 3 分類する

`ResidualKind`(何が起きたか)とは別に `ResidualDisposition`(誰が対処すべきか)を持ちます。
これがないと「残差 1,272 件」という数字が意味を失います。

- **Convertible** — 変換できるはずのものを決定的層が扱えなかった。こちらの宿題。AI 層の対象。
- **ManualMigration** — 設計判断や外部依存。人間が決める。件数が多くても品質の問題ではない。
- **Informational** — 対処不要の通知。

コーパスの残差の大半は ManualMigration です(コーパスが持ち込む依存の量を測っているだけ)。
**追うべきは Convertible の数だけ**です。

### 2.4 認証は fail closed

`Compat/SystemWebStatics.cs` の `Membership.ValidateUser` は常に `false`、
`Roles.IsUserInRole` も常に `false` を返します。`FormsAuthentication.SetAuthCookie` は何もしません。

半分移行したサイトが**誰でも入れる状態になるより、誰も入れない方が安全**だからです。
ここを「とりあえず true」に変えないでください。

### 2.5 ClientID は変換時に静的計算する

WebForms の `ClientIDMode=Predictable`(4.x の既定)を再現します。
ContentPlaceHolder とユーザーコントロールは naming container なので、その ID が
子孫の `ClientID` に連結されます(`cphMain_ctrlWidget_pLabel`)。

Blazor には naming container の概念がないので、**変換時にプレフィックスを算出して
`<WebFormsNamingContainer Prefix="...">` として埋め込みます**。実測で確認済みの細部:

- マスターの自動 ID(`ctl00`)は `ClientID` には出ない(`name` 属性 = `UniqueID` にだけ出る)
- 素の HTML の `id` は決してプレフィックスされない
- ヘッダー/フッターテンプレートは行番号をずらさない

### 2.6 互換コンポーネントの「正直な no-op」

`System.Web` の API を Blazor 上で成立させるため `src/WebForm2Blazor.Components` に
互換層を置いています。対応物が存在しない API は**黙って近似せず、無害な no-op にして
その理由を XML ドキュメントコメントに書く**方針です。例:

- `Page.Master` → `dynamic` で `null`。WebForms ではマスターページのクラス型なので
  `Master.SetStatus(...)` のような呼び出しがコンパイルを通り、最初の使用で明示的に落ちます。
  ビルド全体を止めるより良く、かつ「動いているふり」もしない。
- `EnableViewState` / `ViewStateMode` → 受け取るが何もしない。Blazor はコンポーネントの
  フィールドがそのまま状態なので、切るべき往復ストアが存在しない。
- `HttpApplication` のパイプラインイベント → 購読できるが発火しない。
- `FileIOPermission.Demand()` → 何もしない。CAS は .NET Core で廃止済みで、
  実際のアクセス制御はファイルシステムが 1 行後に行う。

近似すると DOM が変わるもの(`RepeatLayout=Flow`、`ExtractTemplateRows`)は
**残差のまま残します**。これは 2.1 の帰結です。

---

## 3. 現状

### 3.1 構成

```
src/WebForm2Blazor.Converter          変換器        約 8,500 行 / コントロールマッピング 43 種
src/WebForm2Blazor.Components         互換ランタイム 約 8,900 行 / Razor コンポーネント 40 個
tools/WebForm2Blazor.ParityTest       Playwright ゴールデンマスター照合
tools/WebForm2Blazor.BrowserSmokeTest 実ブラウザ操作検証
tools/WebForm2Blazor.PropertyCatalog  実ランタイムから属性カタログ採取
tests/WebForm2Blazor.ConvertedAppTests bUnit
samples/                              変換の実験台 5 つ
```

### 3.2 全体回帰(`tools/verify-all.ps1`、exit 0)

| 段階 | 結果 |
|---|---|
| bUnit | 30 / 30 合格 |
| 変換出力のビルド検証 | 5 アプリすべてエラー 0 件 |
| パリティ(旧アプリ照合) | DefaultsProbe 4 / MasterProbe 3 / ProductAdmin 13 / OrderAdmin 10 = **30 スナップショット全一致** |

パリティは**生 ID 比較が既定**(`ParityScenario.CompareRawIds = true`)です。
ID を正規化して比較すると naming container の実装ミスを取り逃がすので、オプトアウト方式にしてあります。

サンプルの起動ポート(`verify-all.ps1` が使用):

| サンプル | 旧アプリ(IIS Express) | Blazor |
|---|---|---|
| DefaultsProbe | 8093 | 5096 |
| MasterProbe | 8094 | 5097 |
| ProductAdmin | 8091 | 5090 |
| OrderAdmin | 8092 | 5095 |
| HelloWebForms | — | 5080 |

### 3.3 コーパス実測

再現条件を揃えた 5 コーパスの変換結果です。

| コーパス | 移植 .cs | 総残差 | うち Convertible |
|---|---:|---:|---:|
| BlogEngine 3.3.8 | 252 | 83 | 6 |
| mojoPortal 3.1.6 | 569 | 274 | 20 |
| YAF.NET 3.2.15 | 662 | 77 | 5 |
| DNN Platform 9.13.10 | 1,308 | 346 | 12 |
| WingtipToys | 12 | 39 | 3 |
| **計** | | **819** | **46** |

直前の計測は総残差 880 / Convertible 110 だったので、
**Convertible を 110 → 46 に削減**しています。
**生成 Razor の RZ(Razor 構文)エラーは全コーパスで 0 件**です。

nopCommerce 3.8 は Convertible 32 件でしたが、今回は再計測していません(下記 4.3)。

### 3.4 BlogEngine の実ビルドと起動

コーパスの中で唯一「変換出力を実際にビルドして起動まで持っていく」対象にしています。

**ビルドエラー 230 件 → 0 件。変換出力がビルドでき、アプリが起動します。**

```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5099
info: Microsoft.Hosting.Lifetime[0]
      Application started.
```

`/archive` は HTTP 200 で描画され、`blazor.web.js` も配信されています。
ただし `/`・`/post`・`/search`・`/contact` は **HTTP 500** です(下記 4.1)。

**ビルドエラーを 0 にした主な修正**(いずれも変換器・互換層側の一般的な穴):

1. **partial クラスの分断(約 130 件)。** `post.aspx` は `Inherits="post"` で、
   コードビハインドが `Post` 型のメンバを持つため、変換器はコンポーネント名を
   `PostComponent` に退避していました。ところがコードビハインド側はクラス名 `post` のままで、
   `.razor` が生成する `PostComponent` と別クラスになっていた。`@ref` フィールドが全滅し、
   `@inherits` も本来の `BlogBasePage` ではなく既定の `WebFormsPage` になっていました。
   → `ConvertedComponent.SourceClassName`(コードビハインドが実際に宣言している名前)を
   持ち回り、リライタと `ResolveInheritsBase` の双方がそれで検索するようにして解決。

2. **csproj 非参照の孤児ファイル(約 30 件)。** `BlogEngine.Core/Profile.cs` は
   csproj に含まれていないのに `--include` がディレクトリ走査で移植していました。
   C# は同一名前空間の型を using より優先するため、本物の
   `BlogEngine.Core.Data.Models.Profile` を覆い隠していた。
   → `WebFormsProject.ReadCompiledFiles` が `<Compile Include>` を読んで絞り込みます。
   **ただし SDK 形式(`<Project Sdk=...>`)は既定で `**/*.cs` をグロブする**ので、
   明示 Include だけを見ると逆にほぼ全滅します(YAF で 662 → 17 件に激減させた)。
   SDK 形式・ワイルドカード・csproj が一意でない場合は走査にフォールバックします。

3. **コンポーネント名がプロジェクトの型を覆い隠す(5 件 + 波及)。** Razor はコンポーネント名の
   先頭大文字を要求するので `class search` は `Search` になります。BlogEngine には
   `BlogEngine.Core.Search` があり、**元は大文字小文字が違うだけで共存していた**ものが
   衝突しました。しかも影響はその 1 ページに留まらず、同じ名前空間の全ページで
   `Search` がページを指すようになります。
   → `BaseClassRegistry.DeclaresTypeNamed` で衝突を検出し、`SearchComponent` に退避。

4. **`System.Web.UI.Control` の型参照(約 20 件)。** WebForms は単一のコントロール基底を
   持つので `foreach (Control c in ...)` と書いて後で `(TextBox)c` にキャストします。
   互換層はコンポーネント系とレガシー描画系が**兄弟型**なので、このキャストは
   コンパイル不能でした。
   → `CodeBehindRewriter.RewriteControlReferences` が**型参照位置の** `Control` だけを
   `IWebFormsControl` に写します(基底リスト・`new`・`typeof` は対象外)。
   インターフェース経由なら両系統へダウンキャストでき、元コードの意図どおりになります。

5. **除外型のスタブが空だった。** 除外した型のスタブは `class X { }` だけだったので、
   そのメンバを参照する側が壊れていました。
   → **シグネチャが解決できるメンバだけ**を再現するようにしました。const は元のリテラルを
   保持し(`RazorHelpers.PAGE_BODY_MARKER.Length` のように値に依存するコードがある)、
   メソッドは `NotSupportedException` を投げます。解決できない型を含むメンバは出しません
   (コンパイルできないスタブは誰の役にも立たない)。

6. **`<head runat="server">` 内のサーバーコントロールが丸ごと捨てられていた。**
   コードビハインドがそれを参照していてもフィールドが生成されず、参照が壊れました。
   → Blazor の `<HeadContent>` に出力します。副作用として、これまで黙って捨てていた
   未対応コントロールが残差として見えるようになりました(wt +1)。

7. 型忠実性の修正: `Width`/`Height` を `Unit`、`RepeatDirection` を enum、
   `DataKeyNames` を `string[]`、`HttpContext.User` を `IPrincipal`、
   `HtmlTextWriter` を `TextWriter` 派生に。いずれも WebForms 本来の型に合わせたもので、
   ported code がそのままコンパイルできるようになります。

8. `GridView.Columns` をマークアップ用(`ColumnsContent`)と、コードが触るコレクション
   (`Columns`)に分離しました。`grid.Columns.Add(col)` が動きます。

---

## 4. 未解決の問題

### 4.1 BlogEngine はビルド・起動するが、データ層が動かない

ビルドエラーは 0 件で、アプリは起動し `/archive` は描画されます。
一方 `/`・`/post`・`/search`・`/contact` は HTTP 500 で、原因は 1 か所です。

```
System.TypeInitializationException: The type initializer for 'BlogEngine.Core.Right' threw
 ---> System.NullReferenceException
   at BlogEngine.Core.Providers.BlogService.LoadProviders()   BlogService.cs:940
```

```csharp
ProvidersHelper.InstantiateProviders(section.Providers, _providers, typeof(BlogProvider));
```

`section` は `ConfigurationManager.GetSection("BlogEngine")` の戻り値で、互換層は
**null を返します**。プロバイダモデルの設定は Web.config のカスタムセクションにあり、
移行対象外だからです。BlogEngine 側はセクションが存在する前提で `section.Providers` を
読むので、そこで落ちます。

**これは変換器の穴ではなく、設計判断が要る手動移行です。** 埋めるには
`XmlBlogProvider` をどう供給するか(DI 登録にするか、設定を appsettings に移すか)を
決める必要があります。互換層が適当なプロバイダをでっち上げるのは 2.6 の方針に反します。

次の一歩としては、`BlogService` にプロバイダを注入する薄いブートストラップを
`Program.cs` 側に書くのが現実的です。それができれば残りのページも描画されるはずです。

### 4.2 残り 46 件の Convertible 残差

| 件数 | 内容 |
|---:|---|
| 9 | `<script runat="server">` ブロック(コードビハインドへの移動が必要) |
| 7 | `<asp:ListItem resourcekey="...">` で対応する .resx が存在しない(mojo/yaf) |
| 6 | テンプレート外のデータバインド式 |
| 5 | タグ境界を跨ぐインラインコード `<% if (...) { %>` |
| 3 | enum 型パラメータ(`<dnn:Profile EditorMode="View">` など、下記) |
| 2 | `Register` ディレクティブの `Src` に対応する .ascx が無い |
| 14 | その他個別 |

**意図的に残しているもの:**

- **enum 型パラメータ 3 件。** 生成スタブの型が enum かどうか変換時に判定できず、
  当てずっぽうで `@(T.View)` を出すとコンパイルが通らなくなります。
- **`ExtractTemplateRows` 2 件。** 近似すると DOM が変わります(2.6 の方針)。

### 4.3 コーパス測定の再現性

**残差の前後比較は変換オプションを揃えて初めて成立します。** `--input` だけ合わせても比較になりません。
実際に踏んだ罠:

- `--include` を渡さないと YAF の移植 .cs が 662 → 17 になり、`<YAF:LocalizedLabel>` などが
  LegacyRenderHost に解決されず UnmappedControl が 2,000 件超に爆発する
- YAF はサイトルートに `Web.config` が無く、`recommended.web.config` を `--web-config` で
  指定しないと `tagPrefix` を読まない
- mojoPortal は `--control-map` が無いと `<mp:mojoGridView>` が未対応コントロール扱いになる
- WingtipToys の入力ルートは `wingtiptoys-master\WingtipToys\WingtipToys`(1 階層深い)

**判定方法: 変換レポート冒頭の「そのまま移植した .cs」が前回と一致すること。**
一致しなければオプションが違います。

再現手順は `corpora/` にあります(下記 5.1 で解消済み)。

```powershell
.\corpora\fetch.ps1          # コーパス取得(約 115MB)
.\corpora\convert-all.ps1    # 全件変換 + ベースライン比較。一致で exit 0
```

オプションは `convert-all.ps1` に固定してあるので、コマンドラインから崩せません。
詳細は [corpora/README.md](corpora/README.md)。

nopCommerce 1.90 だけは自動取得できません。1.x は CodePlex 時代のリリースで、
GitHub の `nopSolutions/nopCommerce` に 1.x タグが存在せずミラーも見つかりませんでした。
なお以前 `samples/nopCommerce3.8` にあったものは **3.8 = ASP.NET MVC** で、
このコーパス(1.90 = WebForms)とは別物です。272MB あり変換対象にもならないため
Git 管理から外しました(`.gitignore`)。

### 4.4 計測時の落とし穴(実際に誤報告した)

- **インクリメンタルビルドが汚染される。** 変換出力のビルドエラー数を測るときは
  **必ず `--no-incremental`**。ハングしたビルドを kill すると stale な `obj/` が残り、
  「391 → 39 → 3 → 1 件」と報告した後にクリーンビルドで 705 件出たことがあります。
- **出力ディレクトリの残骸。** 前回実行の生成物が残っていると変換レポートが混ざります。
  比較計測は必ず新しい出力先に対して行ってください。
- **PowerShell 5.1 は BOM 無し UTF-8 を ANSI として読む。** 日本語を含む `.ps1` は
  BOM 付き UTF-8 で保存しないと文字化けし、文字列リテラルが壊れます。
- **bUnit と HTML 文字列チェックだけでは不十分。** Blazor Server は「HTML が返る」と
  「操作が効く」の間に JS 配信と SignalR 接続という故障点があります。
  必ず `BrowserSmokeTest`(Playwright + Edge channel)で実ブラウザの操作まで通してください。

### 4.5 README の末尾 2 節が古い

`## 現状の変換実績` の表は DefaultsProbe / MasterProbe が抜けています。
`## 既知の制約` に挙がっている項目のうち、ValidationGroup / RangeValidator /
ListView / FormView / DetailsView / パリティテストは**実装済み**です。更新が必要です。

---

## 5. 次にやること

優先度順です。

### 5.1 コーパス再現環境をリポジトリに入れる — **完了**

`corpora/` に入れました。素の状態(fetch → convert)から 3.3 の数字が再現することを確認済みです。

- `corpora/fetch.ps1` — 5 コーパスを既知のタグで取得
- `corpora/convert-all.ps1` — オプション固定で変換し、ベースラインと比較(差異があれば exit 1)
- `corpora/expected.json` — ベースライン(移植 .cs / 総残差 / 変換可能)
- `corpora/mojo-control-map.json` — mojoPortal 用コントロールマップ
- `corpora/README.md` — 罠と判定の勘所

nopCommerce 1.90 のみ自動取得できません(4.3 参照)。

### 5.2 BlogEngine のデータ層を通す — **ビルドと起動は完了**

ビルドエラー 0、起動 OK、`/archive` は描画。残るのは 4.1 のプロバイダ供給だけです。
`BlogService` にプロバイダを渡すブートストラップを書けば、残りのページも動くはずです。
そこまで行けば「変換ツールが吐いた Blazor アプリが実際にブラウザで動く」最初の実例に
なるので、価値は高いです。実ブラウザでの確認は 4.4 のとおり `BrowserSmokeTest` で。

### 5.3 AI 残差層を実運用する

`AI-TASKS.json` は出力されていますが、**それを LLM に投げて適用するループが未実装**です。
46 件の Convertible 残差が最初の入力になります。
適用後は必ず `verify-all.ps1` で回帰を確認してください。

### 5.4 属性パリティの拡充

2.1 の通り、書かれていない既定値は残差に出ません。`PROPERTY-COVERAGE.md` と
`webforms-property-catalog.json` が唯一の網です。`PropertyCatalog` ツールで
実ランタイムから採取した属性と、実装済みパラメータの差分監査を定期的に回してください。

### 5.5 `--split-projects` を既定にできる状態まで持っていく

方針は決まっています(2026-09-23、ユーザー判断): **分割を既定にし、ベースラインを
分割出力で取り直す。統合出力は `--merge-projects` で残す。** 統合側の不具合を推測で直す
パス(部分修飾名の書き直し)は作らない —— 捨てる予定のモードのためにユーザーのコードを
書き換えることになるため。

既定化の完了条件は「全コーパスで、全ライブラリとアプリ本体がビルドできる」ことです。
ライブラリが 1 つでも落ちるとアプリはコンパイルされず、数字は下限にしかなりません
(検証器が「参照しているプロジェクトが先に失敗」と警告します)。

MVC は互換層で受けることにしました(`Compat/MvcShims.cs`、`System.Web.Mvc` は除外リストから
外した)。将来 MVC プロジェクト自体の変換にも対応する前提です。

**現在地(n2cms の N2.dll 単体): 225 → 14 → 10。**

**互換層の 2 系統は統合できる(試作済み、2026-09-23)。** `LegacyWebControl` を
`WebFormsControlBase` の下に付け替えて 1 本の継承にし、`DropDownList` を `ListControl` の
下に移した。53 メンバーの重複はほぼ `virtual` の有無だけで、互換層内の修正は 2 箇所だった。
bUnit・パリティ・be/wt は一切動かず、n2 の `new DropDownList()` を `ListControl` で返す 4 件が
消えた。コードで作った `DropDownList` を描画型の親に入れても、要素の内側に本物の
コンポーネントとして描かれ、選択がサーバーに戻る(`UnifiedControlHierarchyTests`)。

仕組み: 描画型は `BuildRenderTree` から自分の `Render` を実行する。子にコンポーネントが
いるときだけ、書き出しに印を置いて HTML を要素フレームに組み立て直し、印の位置に本物の
コンポーネントを置く(`Compat/MarkupFrames.cs`)。文字列を分割して差し込むと、ブラウザは
開始タグ単体を空の要素として閉じてしまうため。**bUnit は全フレームの HTML をつないで
解析するので、この違いを検出できない** —— フレームを直接見るテストを置いてある。

次: 同じ手順を他の部品へ(`TextBox`、各検証器が n2 の残り 3 件)。

| 群 | 件数 | 中身 | 論点 |
|---|---:|---|---|
| 互換層の 2 系統分岐 | 3 | `RangeValidator`/`CompareValidator` を `BaseValidator` として返す / `FreeTextArea`→`TextBox` | 統合方式で解決できる(上記)。部品ごとに移す |
| .NET が削除した API | 7 | n2 同梱 Castle DynamicProxy の CAS・`RunAndSave`・`AssemblyBuilder.Save` | **解消(n2 分割で N2.dll エラー 0、2026-09-23 計測)**。変換器 `Convert/RemovedEmitApis.cs`(`AppDomain.DefineDynamicAssembly` → `AssemblyBuilder.DefineDynamicAssembly`、`RunAndSave` → `Run`、`DefineDynamicModule` は名前だけ、`Save` は `PlatformNotSupportedException` + 残差)。互換層の権限クラスを `IPermission` に(`InertCodeAccessPermission`)。`using System.Security.Permissions` を外すと消えていた `PermissionState` を別名で補う。同型の検証用サンプル `samples/DefaultsProbe/ProbeEmit.cs` を変換し、実行時に型を生成できることを `RemovedEmitApiTests` で確認 |

**n2 の現状(分割、2026-09-23)**: N2.dll はビルドが通り、変換後の N2.dll で n2 自身の
`ProxyGenerator` が `ContentItem` のサブクラスのプロキシを生成・インターセプトできることを確認した
(`[NonInterceptable]` の付いた `Title` 等が対象外なのは n2 の設計どおり)。その後 `N2.Management`(50 件)/ `N2.Extensions`(5 件)も
解消し、分割モードで初めてアプリ本体 `N2Templates` のコンパイルに届いた(34 件、下限ではない)。
直したのは互換層の宣言不足(設定セクション群を `ConfigurationSection` 派生に、`FormsAuthentication
.Authenticate` は fail closed、`HashPasswordForStoringInConfigFile` は本物のハッシュ、MVC の `ActionLink`、
`ObjectDataSource.Delete`、`Control.Parent`、`LegacyImage`、コードビハインドの `Eval(式)` =
`DataItemScope`)と、変換器の 2 件: 名前空間と同名のフィールドを互換層の型に書き換えていた
(`CompatImportDisambiguator`。式の中ではメンバー・ローカルが名前空間より優先される)、
`Color` を色プロパティへ代入するコードの文字列化(`ColorAssignmentRewriter`)。
`CreateUserStep.CustomNavigationTemplateContainer` だけは実装できず null(互換 Wizard のボタンは
コントロールではない)。n2 の新規ユーザー画面はその行で落ちる。

その過程で見つけた **C# 14 の `field` キーワード問題**: アクセサー内の `field` が C# 14 では
自動生成の裏側のフィールドを指すため、メンバー名が `field` のコード(DynamicProxy の
`FieldReference`)がエラーも出さずに null を返していた。変換器が `@field` に書き換える
(`Convert/FieldKeywordRewriter.cs`。`<script runat="server">` の経路も対象)。コンパイルが通って
意味だけ変わる種類なので、ビルドエラー数には現れない。

n2 以外(分割時・MVC 互換層を入れる前の値): yaf 3(変換器がライブラリにアプリ名前空間の
`using` を挿入。`Convert\RelocatedTypeIndex.cs:194` が本命)/ dnn 7 / mojo 12。

**注意: mojo / yaf / dnn / n2 の `expected.json` は MVC 互換層より前の数字です。**
コーパス検証は be / wt に限る決まり(`CLAUDE.md`)なので、この 4 本は測り直していません。
次にその 4 本を測る指示があったときに、ベースラインを取り直してください。

**イベントの名前の約束**(段階 (b) の `Button` を止めていた `OnClick` の衝突の解消)。
WebForms のイベント X には「マークアップの `OnX="…"`」「コードの `X += …`」「サブクラスが
上書きする保護メソッド `OnX(EventArgs)`」の 3 経路があり、C# では引数 `OnX` とメソッド
`OnX` を同じクラスに置けない。約束はこう決めた:

| 経路 | 互換層の名前 |
|---|---|
| マークアップ | `[Parameter] OnX`(WebForms の綴りのまま。変換後のマークアップは変わらない) |
| コード | CLR イベント `X`(引数とは別の置き場所) |
| サブクラスの上書き | `protected virtual XHandler(...)`。マークアップ分 → コード分の順に呼ぶ |

変換器は、移植クラスの `override OnX` と `base.OnX(...)` / クラス内の `OnX(...)` を `XHandler`
に改名する(`CodeBehindRewriter.RenameRaiseMethods`)。判定は互換アセンブリに聞く:
基底に `XHandler` メソッドがあり `OnX` メソッドが無いときだけ。`LegacyButton` などの
描画型の基底はまだ `OnClick` のままなので触らない —— 段階 (b) でそれらを Razor 部品の下へ
移し、メソッド名を `XHandler` にすれば、改名は自動で付いてくる。

直したもの: (1) 以前はコードの `X += h` を引数 `OnX` に足し込んでいたため、マークアップにも
`OnX` があると親の再描画で引数が代入し直され、コード側のハンドラが黙って消えていた
(bUnit で再現、`EventNamingTests`)。(2) 対応表に無かった `TextBox` の `OnTextChanged` と
`GridView` の `OnRowUpdating`/`OnRowCancelingEdit`/`OnRowCreated` が、イベントではなく
HTML 属性として書き出されていた。(3) 「ハンドラが無ければ既定動作」の判定
(`GridView` のページ送り・並べ替え、`ChangePassword`/`PasswordRecovery`、`Timer`、
`ItemDataBound` 系)がマークアップ分しか見ていなかった。

残したもの: `Calendar.OnDayRender` は互換層の形(`Action<CalendarDay>`)が WebForms
(`TableCell` と `CalendarDay`)と違い、Razor 版はセルを持たないので `Calendar` の段階 (b) で扱う。
また、ported 基底を何段か挟んだクラスの override 判定(`CompatDeclares`)は制御の基底を
`LegacyWebControl` とみなす近似のまま(実際に出力する基底は `CompatBaseReplacements` の値)。
直すと検証できない 4 コーパスの数字が動きうるので、今回は改名分を判定から外すだけにした。

同種の見落としを探す道具として、変換器の辞書初期化子の重複キー検査を一度流しました
(`CompatBaseReplacements` で `CheckBoxList` / `RadioButtonList` が後勝ちで上書きされ、
`LegacyListControl` への対応が無効になっていた)。辞書を足したら同じ検査を流す価値があります。
### 5.6 CI

現状 `verify-all.ps1` は手元でしか回りません。IIS Express と .NET Framework の
MSBuild が要るのでパリティ層は Windows ランナー必須です。
最低限 bUnit + `--verify-build` だけでも CI に載せる価値があります。

---

## 6. 引き継ぎメモ

- **コミット前に必ず `tools/verify-all.ps1` を通す。** exit 0 が最低ライン。
- **変換器を触ったら `corpora/convert-all.ps1` も通す。** こちらも exit 0 が最低ライン。
- 互換コンポーネントに API を足すときは、**動く実装か、理由を書いた no-op か**の
  どちらかにしてください。中途半端な近似が一番たちが悪い。
- 数字が動いたら、**まず「移植 .cs」を見る。** そこが動いていれば原因は変換器ではなく
  オプションかコーパス側です(4.3)。
- 迷ったら「旧アプリが実際に何を出しているか」を見に行く。それが唯一の正解です。
