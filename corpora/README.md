# コーパス

変換の適用範囲は、実在する WebForms アプリを変換したときの**残差の件数**で測ります。
その測定対象をここで取得・変換・比較します。

コーパス本体はサードパーティのソースで、それぞれ独自のライセンスを持ちます。
リポジトリには含めず、`fetch.ps1` が既知のバージョンを `corpora/work/` に取得します
(`corpora/work/` と `corpora/out/` は `.gitignore` 済み)。

## 使い方

```powershell
.\corpora\fetch.ps1          # コーパスを取得(初回のみ、約 115MB)
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
| `wt` | WingtipToys | `corn-mendoza/wingtiptoys` @ `master` |

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
| `wt` | なし(入力ルートが 1 階層深い) | — |

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
be            252      83         6             0
mojo          729     265        20           599
yaf          2722      80         5           188
dnn          1944     392        12          1086
wt             12      39         3            16
合計                  859        46          1889
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

## 既知の注意点

- 出力は毎回作り直されます。前回の生成物が残っているとレポートが混ざり、
  「直したはずの残差がまだ出る」ように見えます(実際に一度これで誤読しました)。
- `wt` の変換出力をビルドすると `NVPAPICaller` などが未解決になりますが、これは
  **コーパス側にそのファイルが存在しない**ためで、変換器の問題ではありません。
- これらのスクリプトは PowerShell 5.1 互換です。日本語を含むため
  **BOM 付き UTF-8** で保存してください(BOM 無しだと PS 5.1 は ANSI として読み、
  文字列リテラルが壊れます)。
