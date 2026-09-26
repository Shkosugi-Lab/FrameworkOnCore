# 実験: WebFormsForCore で元の Web Forms をそのまま動かす(2026-09-26)

方針転換(Blazor 化ではなく「.NET Framework アプリを Linux で動かす」)の最初の試作。
サンプルのソースは変更せず、SDK 形式のプロジェクト(`template.csproj.txt`)と `Program.cs`
(`Program.cs.txt`)だけを足して、WebFormsForCore 1.6.4(NuGet)で .NET 10 上に載せる。
比較相手は IIS / .NET Framework で採った正解データ(`samples/*/golden-webforms.json`)。

    .\experiments\wf4c\run-sample.ps1 -Name ProductAdmin
    .\experiments\wf4c\run-sample.ps1 -Name DefaultsProbe -Exclude ProbeEmit.cs

## 結果(Windows 上。Linux(Docker)は未実施)

| サンプル | 結果 |
|---|---|
| ProductAdmin | **13/13 一致** |
| OrderAdmin(UpdatePanel・全バリデータ) | **10/10 一致** |
| MasterProbe(入れ子マスター) | **3/3 一致** |
| DefaultsProbe | **4/4 一致**(フォーク 0001 適用後) |

## 見つかったこと

- **DynamicData が NuGet に無い。** `ItemType` を持つデータバインドコントロールは OnInit で
  `DataBoundControlHelper.EnableDynamicData` を呼び、System.Web.DynamicData を読み込む。上流の
  WebFormsForCore.Web.DynamicData は LINQ to SQL / EF6 に依存して未完成でビルドできない。
  必要なのは EnableDynamicData の一部なので、ランタイム側で直せる見込み(フォークの必要性の具体例)。
- **既定のドキュメント。** IIS の「/ → Default.aspx」が Kestrel に無い。`UseDefaultFiles` で補う
  (`Program.cs.txt`)。変換器が生成すべき項目。
- **README の手順どおりでは動かない箇所がある。** `System.Configuration.ConfigurationManager` を
  参照から外す手順は、WebFormsForCore.Configuration(同名の DLL)まで外してしまう。
  `\dotnet\` の正規表現は Windows の MSBuild で評価できない(`[/\x5C]dotnet[/\x5C]` に変更)。
- **.NET Framework → .NET の API 差は変換器の仕事のまま。** DefaultsProbe の `ProbeEmit.cs`
  (Reflection.Emit の削除 API)は元のままではコンパイルできない。旧変換器の `RemovedEmitApis`
  などが流用できる。
- **上流のソースからのビルド**は `src/WebFormsForCore.Build` を先にビルドする必要がある
  (`lib/WebFormsForCore.Build/.../FakeStrongName.targets` を出力する)。

`_upstream/` は上流の浅いクローン(Git 管理外)。

## フォーク(2026-09-26、`_upstream` のローカルブランチ `w2l/dynamicdata`)

上流 WebFormsForCore の main(1.6.4 相当)に対する修正。`patches/` に `git format-patch` の形で置く。
`pack-fork.ps1` で `1.6.5-w2l.1` として `_feed/` にパッケージ化し、テンプレートはそれを参照する。

| パッチ | 内容 | 必要になった場面 |
|---|---|---|
| 0001 | DynamicData を .NET でビルド(EF6 / LINQ to SQL のモデルプロバイダーを除外、ScaffoldTableAttribute) | `ItemType` 付きのデータバインドコントロール(DefaultsProbe、wt) |
| 0002 | `Response.Headers` を IIS7 以外でも使えるように(クラシックの header リストと同期) | OWIN(Microsoft.Owin.Host.SystemWeb)。ASP.NET Identity を使う 4.5 以降のテンプレート |
| 0003 | ページのコンパイルに mscorlib / netstandard のファサードを参照 | .NET Framework 向けライブラリを呼ぶページ(wt の Site.Master → Identity) |

未対応: VB のページコンパイラー(`VBCompiler.cs`)にも 0003 と同じ対応が要る(VB 対応のときに)。

## 互換アセンブリ(`shims/`)

.NET Framework にあって .NET に無いアセンブリを、.NET Framework 向けパッケージが参照している場合に置く。
公開鍵トークンが違っても名前とバージョンで解決される(DynamicData で確認)。

- `System.Net.Http.WebRequest`(`WebRequestHandler`): Katana の Microsoft.Owin.Security.*

## wt(WingtipToys、実在の OSS)

`convert-project.ps1` で旧形式の csproj から SDK 形式を作る(ソースは一切変更しない)。

    .\experiments\wf4c\convert-project.ps1 -Project corpora\work\wingtiptoys-master\WingtipToys\WingtipToys\WingtipToys.csproj -Out experiments\wf4c\wt

結果: **8 画面中 6 一致**。残り 2 は既知の丸めの差(`double` 22.5 の通貨書式が .NET Framework は ¥23、
.NET は ¥22)で、アプリのコード側の差。EF6 6.5.1、ASP.NET Identity 2 + OWIN(.NET Framework 版のまま)、
FriendlyUrls、Elmah、バンドルがそのまま動いている。

変換スクリプトに入れた規則(新しい変換器に持ち込むもの):
- packages.config → PackageReference。.NET に同梱の System.* 4.x 等は外す。WebFormsForCore に同等品が
  あるもの(バンドル、WebGrease、Microsoft.Web.Infrastructure、AjaxControlToolkit)は置き換える。
  EF6 は 6.5.1、Newtonsoft.Json は依存先の要求(13.0.4)まで上げる(下げると NU1605)。
- 元の `<Compile Include>` をそのまま使う。`GenerateAssemblyInfo=false`(元の AssemblyInfo.cs と重複するため)。
- ルーティング(RouteTable / FriendlyUrls)を使うアプリは `HandleAllRequestsWithWebForms()`。
  使わないアプリは IIS の既定ドキュメント(`/` → Default.aspx)を `UseDefaultFiles` で補う。

ParityTest の修正: 拡張子なしのパスを HTTP で事前確認する方式をやめた(GET で Web Forms のページが
実行されるため、AddToCart が 2 回実行されてカートが 2 件になった)。開いて失敗したら .aspx で開き直す。
