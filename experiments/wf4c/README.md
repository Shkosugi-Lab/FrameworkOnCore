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
| DefaultsProbe | 実行時エラー: `ItemType` 付き ListView が `System.Web.DynamicData` を要求 |

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
