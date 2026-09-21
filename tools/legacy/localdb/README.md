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
