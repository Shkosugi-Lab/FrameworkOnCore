# LocalDB のインストーラ

`wt`(WingtipToys)の**変換前アプリ**を IIS で動かすために要ります。
このセッションの実行環境からは `msiexec` が通らなかったので、**対話セッションから実行してください**。

## 置いてあるもの

`.msi` は `.gitignore` 済みです(`nuget.exe` と同じ「取得する道具」の扱い)。
消えていたら下の URL から取り直せます。

| ファイル | サイズ | SHA256 |
|---|---:|---|
| `SqlLocalDB-2019.msi` | 55,762,944 | `80702F2B2732DE041FE34EA6477EA556EEB5D73EE44CE4A4301DE719455345E0` |
| `SqlLocalDB-2022.msi` | 63,508,480 | `224D483992EF60368DAC70CEA174DCFAF43A3CA06ADA331C67DC6119A26490F6` |

取得元:

- 2019: `https://download.microsoft.com/download/7/c/1/7c14e92e-bdcb-4f89-b7cf-93543e7112d1/SqlLocalDB.msi`
- 2022: `https://download.microsoft.com/download/3/8/d/38de7036-2433-4207-8eae-06e247e17b25/SqlLocalDB.msi`

**2019 を勧めます。** WingtipToys は EF6 の古いアプリで、そちらのほうが素直です。

## インストール

管理者権限の PowerShell から:

```powershell
msiexec /i .\SqlLocalDB-2019.msi /qn IACCEPTSQLLOCALDBLICENSETERMS=YES
```

うまくいかないときはログを取ってください。

```powershell
msiexec /i .\SqlLocalDB-2019.msi /qn IACCEPTSQLLOCALDBLICENSETERMS=YES /l*v install.log
```

確認:

```powershell
sqllocaldb info          # MSSQLLocalDB が出れば入っています
sqllocaldb start MSSQLLocalDB
```

## このセッションで止まった場所(再試行の参考に)

2019 / 2022 のどちらも同じところで落ちました。

```
Action ended: InstallFinalize. Return value 3.
Note: 1: 2265 2:  3: -2147287035        ← STG_E_ACCESSDENIED
終了コード 1603
```

管理者権限はあり(`IsInRole(Administrator)` = True)、サンドボックスを外しても同じでした。
**2 バージョンが同じ箇所で落ちる**ので、パッケージ側ではなく実行環境側の制約と見ています。

## 入ったあと

`corpora/README.md` の「LocalDB のインストールを試みて…」の節に続きの手順があります。要点だけ:

1. `wt` の元アプリのビルドは**もう通ります**(`corpora/build-original.ps1 -Only wt`)
2. 接続文字列には、元アプリが**本来つながるはずだったデータベースを与えてください**。
   起動させるために設定を書き換えて「つながらないことにする」のは**やらないでください** ——
   採れたものが変換前アプリでなくなり、ゲートの土台が崩れます
3. `corpora/record-webforms-golden.ps1` の `$targets` に足す
   (`Name = 'wt'`、`Path = 'wingtiptoys-master\WingtipToys\WingtipToys'`、`Port = 8092`)
4. `corpora/parity-gate.ps1` の `$targets` にも足す(`Port = 5080`)

`wt` はいまビルドエラー 0・回帰 8/8 で「動いて見えて」います。
`be` が照合前にいたのと同じ場所です。

---

## 追記: 失敗の原因は 2 つあった(実測で特定)

### 1. パスに `-` が含まれると msiexec は MSI を開けない

```
msiexec /i .\SqlLocalDB-2019.msi /qn ...      ← このリポジトリ上で実行すると開けない
```

イベントログ(Application / MsiInstaller)に `Beginning`/`Ending` は出るのに
`installed the product` が出ません。**エラー 1619(パッケージを開けなかった)**です。
`/qn` なので画面には何も出ず、**成功したように見えます**。

原因はリポジトリのパス `C:\wcc\data\sessions\-rETZO4pVnOb\...` —— `-` で始まるディレクトリ名を
`msiexec` が引数と解釈します。**必ず単純なパスにコピーしてから実行してください。**

```powershell
copy .\SqlLocalDB-2019.msi $env:TEMP\ldb2019.msi
msiexec /i $env:TEMP\ldb2019.msi /qn IACCEPTSQLLOCALDBLICENSETERMS=YES /l*v $env:TEMP\ldb.log
```

### 2. 本当の原因は SQL Writer サービスの起動タイムアウト

単純なパスから実行すると MSI は開けます。そこで出るのがこれです。

```
Error 1920. Service 'SQL Server VSS Writer' (SQLWriter) failed to start.
The service did not respond to the start or control request in a timely fashion.
A timeout was reached (30000 milliseconds)
```

2019 / 2022 とも同じ。`ADDLOCAL` / `REMOVE` で `SQL_WRITER_LocalDB` を外そうとしても、
親 Feature の子なので一緒に入り、同じ場所で落ちます。

**対処**: サービス起動の待ち時間を延ばして再起動します。

```powershell
Set-ItemProperty "HKLM:\SYSTEM\CurrentControlSet\Control" -Name ServicesPipeTimeout -Value 180000 -Type DWord
# 再起動が必要（この設定は起動時にしか読まれません）
```

**設定済みです**(180000 ms = 3 分)。再起動後に上のコマンドでインストールしてください。

### 外した推測(同じ道を歩かないために)

`STG_E_ACCESSDENIED`(`Note: 1: 2265 3: -2147287035`)というコードが出ていたので、
そこから 2 回推測して 2 回とも外しました。

| 推測 | 実測 |
|---|---|
| 「Windows コンテナの中だから」 | **違う**。`wcifs` のインスタンスは 0、実機の Windows 11 Pro build 26200(Intel NUC) |
| 「Defender がブロックしている」 | **違う**。CFA 無効・ASR ルールなし・検出履歴なし |

**イベントログを見れば最初から `Error 1920` と書いてありました。**
MSI のログに出る `2265` はロールバックに伴う二次的なもので、原因ではありませんでした。
`msiexec` の終了コードだけを見て推測を広げたのが誤りです。
**まず `Get-WinEvent` で Application / System を見てください。**
