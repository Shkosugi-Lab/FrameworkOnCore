# WebForms プロパティカバレッジ監査

- 既定値カタログの採取元ランタイム: .NET Framework CLR 4.0.30319.42000(実機から採取)
- 「対応」= マッピング表(マークアップ属性)または互換コンポーネントの公開 API(コードビハインド)でカバー
- 「無害」= Blazor では意味を持たないため除去してよい既知の属性

**総計: 対応 474 / 未対応 360**

未対応プロパティは、マークアップで使われれば残差レポート、コードビハインドで使われれば
Roslyn 使用棚卸し(変換時)とコンパイルエラーで表面化する。この監査はそれを「使われる前に」可視化する。

## BoundField(対応 4 / 無害 0 / 未対応 13)

| 未対応プロパティ | 型 | 実機の既定値 |
| --- | --- | --- |
| AccessibleHeaderText | String | `""` |
| ApplyFormatInEditMode | Boolean | `False` |
| ConvertEmptyStringToNull | Boolean | `True` |
| FooterText | String | `""` |
| HeaderImageUrl | String | `""` |
| HtmlEncode | Boolean | `True` |
| HtmlEncodeFormatString | Boolean | `True` |
| InsertVisible | Boolean | `True` |
| NullDisplayText | String | `""` |
| ReadOnly | Boolean | `False` |
| ShowHeader | Boolean | `True` |
| ValidateRequestMode | ValidateRequestMode | `Inherit` |
| Visible | Boolean | `True` |

## Button(対応 20 / 無害 6 / 未対応 7)

| 未対応プロパティ | 型 | 実機の既定値 |
| --- | --- | --- |
| AppRelativeTemplateSourceDirectory | String | `""` |
| Page | Page | `null` |
| PostBackUrl | String | `""` |
| RenderingCompatibility | Version | `(複合型: Version)` |
| Site | ISite | `null` |
| TemplateControl | TemplateControl | `null` |
| ValidateRequestMode | ValidateRequestMode | `Inherit` |

## CheckBox(対応 17 / 無害 6 / 未対応 8)

| 未対応プロパティ | 型 | 実機の既定値 |
| --- | --- | --- |
| AppRelativeTemplateSourceDirectory | String | `""` |
| CausesValidation | Boolean | `False` |
| Page | Page | `null` |
| RenderingCompatibility | Version | `(複合型: Version)` |
| Site | ISite | `null` |
| TemplateControl | TemplateControl | `null` |
| ValidateRequestMode | ValidateRequestMode | `Inherit` |
| ValidationGroup | String | `""` |

## CheckBoxList(対応 21 / 無害 7 / 未対応 19)

| 未対応プロパティ | 型 | 実機の既定値 |
| --- | --- | --- |
| AppRelativeTemplateSourceDirectory | String | `""` |
| CausesValidation | Boolean | `False` |
| CellPadding | Int32 | `-1` |
| CellSpacing | Int32 | `-1` |
| DataMember | String | `""` |
| DataSourceID | String | `""` |
| DataTextFormatString | String | `""` |
| ItemType | String | `""` |
| Page | Page | `null` |
| RenderWhenDataEmpty | Boolean | `False` |
| RenderingCompatibility | Version | `(複合型: Version)` |
| RepeatColumns | Int32 | `0` |
| SelectMethod | String | `""` |
| Site | ISite | `null` |
| TemplateControl | TemplateControl | `null` |
| Text | String | `""` |
| TextAlign | TextAlign | `Right` |
| ValidateRequestMode | ValidateRequestMode | `Inherit` |
| ValidationGroup | String | `""` |

## CompareValidator(対応 23 / 無害 7 / 未対応 9)

| 未対応プロパティ | 型 | 実機の既定値 |
| --- | --- | --- |
| AppRelativeTemplateSourceDirectory | String | `""` |
| AssociatedControlID | String | `""` |
| CultureInvariantValues | Boolean | `False` |
| IsValid | Boolean | `True` |
| Page | Page | `null` |
| RenderingCompatibility | Version | `(複合型: Version)` |
| Site | ISite | `null` |
| TemplateControl | TemplateControl | `null` |
| ValidateRequestMode | ValidateRequestMode | `Inherit` |

## CustomValidator(対応 20 / 無害 8 / 未対応 8)

| 未対応プロパティ | 型 | 実機の既定値 |
| --- | --- | --- |
| AppRelativeTemplateSourceDirectory | String | `""` |
| AssociatedControlID | String | `""` |
| IsValid | Boolean | `True` |
| Page | Page | `null` |
| RenderingCompatibility | Version | `(複合型: Version)` |
| Site | ISite | `null` |
| TemplateControl | TemplateControl | `null` |
| ValidateRequestMode | ValidateRequestMode | `Inherit` |

## DataList(対応 23 / 無害 6 / 未対応 22)

| 未対応プロパティ | 型 | 実機の既定値 |
| --- | --- | --- |
| AppRelativeTemplateSourceDirectory | String | `""` |
| Caption | String | `""` |
| CaptionAlign | TableCaptionAlign | `NotSet` |
| DataKeyField | String | `""` |
| DataMember | String | `""` |
| DataSourceID | String | `""` |
| EditItemIndex | Int32 | `-1` |
| EditItemTemplate | ITemplate | `null` |
| ExtractTemplateRows | Boolean | `False` |
| GridLines | GridLines | `None` |
| HorizontalAlign | HorizontalAlign | `NotSet` |
| Page | Page | `null` |
| RenderingCompatibility | Version | `(複合型: Version)` |
| SelectedIndex | Int32 | `-1` |
| SelectedItemTemplate | ITemplate | `null` |
| SeparatorTemplate | ITemplate | `null` |
| ShowFooter | Boolean | `True` |
| ShowHeader | Boolean | `True` |
| Site | ISite | `null` |
| TemplateControl | TemplateControl | `null` |
| UseAccessibleHeader | Boolean | `False` |
| ValidateRequestMode | ValidateRequestMode | `Inherit` |

## DropDownList(対応 20 / 無害 6 / 未対応 14)

| 未対応プロパティ | 型 | 実機の既定値 |
| --- | --- | --- |
| AppRelativeTemplateSourceDirectory | String | `""` |
| CausesValidation | Boolean | `False` |
| DataMember | String | `""` |
| DataSourceID | String | `""` |
| DataTextFormatString | String | `""` |
| ItemType | String | `""` |
| Page | Page | `null` |
| RenderingCompatibility | Version | `(複合型: Version)` |
| SelectMethod | String | `""` |
| Site | ISite | `null` |
| TemplateControl | TemplateControl | `null` |
| Text | String | `""` |
| ValidateRequestMode | ValidateRequestMode | `Inherit` |
| ValidationGroup | String | `""` |

## FileUpload(対応 14 / 無害 5 / 未対応 7)

| 未対応プロパティ | 型 | 実機の既定値 |
| --- | --- | --- |
| AllowMultiple | Boolean | `False` |
| AppRelativeTemplateSourceDirectory | String | `""` |
| Page | Page | `null` |
| RenderingCompatibility | Version | `(複合型: Version)` |
| Site | ISite | `null` |
| TemplateControl | TemplateControl | `null` |
| ValidateRequestMode | ValidateRequestMode | `Inherit` |

## FormView(対応 16 / 無害 5 / 未対応 35)

| 未対応プロパティ | 型 | 実機の既定値 |
| --- | --- | --- |
| AllowPaging | Boolean | `False` |
| AppRelativeTemplateSourceDirectory | String | `""` |
| BackImageUrl | String | `""` |
| Caption | String | `""` |
| CaptionAlign | TableCaptionAlign | `NotSet` |
| CellPadding | Int32 | `-1` |
| CellSpacing | Int32 | `0` |
| DataKeyNames | String[] | `(複合型: String[])` |
| DataMember | String | `""` |
| DataSourceID | String | `""` |
| DefaultMode | FormViewMode | `ReadOnly` |
| DeleteMethod | String | `""` |
| EditItemTemplate | ITemplate | `null` |
| EmptyDataTemplate | ITemplate | `null` |
| EmptyDataText | String | `""` |
| EnableModelValidation | Boolean | `True` |
| FooterTemplate | ITemplate | `null` |
| FooterText | String | `""` |
| GridLines | GridLines | `None` |
| HeaderTemplate | ITemplate | `null` |
| HeaderText | String | `""` |
| HorizontalAlign | HorizontalAlign | `NotSet` |
| InsertItemTemplate | ITemplate | `null` |
| InsertMethod | String | `""` |
| ItemType | String | `""` |
| Page | Page | `null` |
| PageIndex | Int32 | `0` |
| PagerTemplate | ITemplate | `null` |
| RenderOuterTable | Boolean | `True` |
| RenderingCompatibility | Version | `(複合型: Version)` |
| SelectMethod | String | `""` |
| Site | ISite | `null` |
| TemplateControl | TemplateControl | `null` |
| UpdateMethod | String | `""` |
| ValidateRequestMode | ValidateRequestMode | `Inherit` |

## GridView(対応 27 / 無害 5 / 未対応 34)

| 未対応プロパティ | 型 | 実機の既定値 |
| --- | --- | --- |
| AllowCustomPaging | Boolean | `False` |
| AppRelativeTemplateSourceDirectory | String | `""` |
| AutoGenerateDeleteButton | Boolean | `False` |
| AutoGenerateEditButton | Boolean | `False` |
| AutoGenerateSelectButton | Boolean | `False` |
| BackImageUrl | String | `""` |
| CaptionAlign | TableCaptionAlign | `NotSet` |
| ClientIDRowSuffix | String[] | `(複合型: String[])` |
| ColumnsGenerator | IAutoFieldGenerator | `null` |
| DataMember | String | `""` |
| DataSourceID | String | `""` |
| DeleteMethod | String | `""` |
| EditIndex | Int32 | `-1` |
| EmptyDataTemplate | ITemplate | `null` |
| EnableModelValidation | Boolean | `True` |
| EnablePersistedSelection | Boolean | `False` |
| EnableSortingAndPagingCallbacks | Boolean | `False` |
| HorizontalAlign | HorizontalAlign | `NotSet` |
| ItemType | String | `""` |
| Page | Page | `null` |
| PagerTemplate | ITemplate | `null` |
| RenderingCompatibility | Version | `(複合型: Version)` |
| RowHeaderColumn | String | `""` |
| SelectMethod | String | `""` |
| SelectedIndex | Int32 | `-1` |
| SelectedPersistedDataKey | DataKey | `null` |
| ShowFooter | Boolean | `False` |
| ShowHeaderWhenEmpty | Boolean | `False` |
| Site | ISite | `null` |
| TemplateControl | TemplateControl | `null` |
| UpdateMethod | String | `""` |
| UseAccessibleHeader | Boolean | `True` |
| ValidateRequestMode | ValidateRequestMode | `Inherit` |
| VirtualItemCount | Int32 | `0` |

## HiddenField(対応 3 / 無害 5 / 未対応 6)

| 未対応プロパティ | 型 | 実機の既定値 |
| --- | --- | --- |
| AppRelativeTemplateSourceDirectory | String | `""` |
| Page | Page | `null` |
| RenderingCompatibility | Version | `(複合型: Version)` |
| Site | ISite | `null` |
| TemplateControl | TemplateControl | `null` |
| ValidateRequestMode | ValidateRequestMode | `Inherit` |

## HyperLink(対応 17 / 無害 5 / 未対応 9)

| 未対応プロパティ | 型 | 実機の既定値 |
| --- | --- | --- |
| AppRelativeTemplateSourceDirectory | String | `""` |
| ImageHeight | Unit | `Unit.Empty` |
| ImageUrl | String | `""` |
| ImageWidth | Unit | `Unit.Empty` |
| Page | Page | `null` |
| RenderingCompatibility | Version | `(複合型: Version)` |
| Site | ISite | `null` |
| TemplateControl | TemplateControl | `null` |
| ValidateRequestMode | ValidateRequestMode | `Inherit` |

## Image(対応 17 / 無害 5 / 未対応 8)

| 未対応プロパティ | 型 | 実機の既定値 |
| --- | --- | --- |
| AppRelativeTemplateSourceDirectory | String | `""` |
| DescriptionUrl | String | `""` |
| GenerateEmptyAlternateText | Boolean | `False` |
| Page | Page | `null` |
| RenderingCompatibility | Version | `(複合型: Version)` |
| Site | ISite | `null` |
| TemplateControl | TemplateControl | `null` |
| ValidateRequestMode | ValidateRequestMode | `Inherit` |

## Label(対応 16 / 無害 5 / 未対応 6)

| 未対応プロパティ | 型 | 実機の既定値 |
| --- | --- | --- |
| AppRelativeTemplateSourceDirectory | String | `""` |
| Page | Page | `null` |
| RenderingCompatibility | Version | `(複合型: Version)` |
| Site | ISite | `null` |
| TemplateControl | TemplateControl | `null` |
| ValidateRequestMode | ValidateRequestMode | `Inherit` |

## LinkButton(対応 20 / 無害 5 / 未対応 7)

| 未対応プロパティ | 型 | 実機の既定値 |
| --- | --- | --- |
| AppRelativeTemplateSourceDirectory | String | `""` |
| Page | Page | `null` |
| PostBackUrl | String | `""` |
| RenderingCompatibility | Version | `(複合型: Version)` |
| Site | ISite | `null` |
| TemplateControl | TemplateControl | `null` |
| ValidateRequestMode | ValidateRequestMode | `Inherit` |

## ListItem(対応 3 / 無害 0 / 未対応 1)

| 未対応プロパティ | 型 | 実機の既定値 |
| --- | --- | --- |
| Enabled | Boolean | `True` |

## ListView(対応 20 / 無害 5 / 未対応 31)

| 未対応プロパティ | 型 | 実機の既定値 |
| --- | --- | --- |
| AppRelativeTemplateSourceDirectory | String | `""` |
| ClientIDRowSuffix | String[] | `(複合型: String[])` |
| ConvertEmptyStringToNull | Boolean | `True` |
| DataKeyNames | String[] | `(複合型: String[])` |
| DataMember | String | `""` |
| DataSourceID | String | `""` |
| DeleteMethod | String | `""` |
| EditIndex | Int32 | `-1` |
| EditItemTemplate | ITemplate | `null` |
| EmptyItemTemplate | ITemplate | `null` |
| EnableModelValidation | Boolean | `True` |
| EnablePersistedSelection | Boolean | `False` |
| GroupItemCount | Int32 | `1` |
| GroupPlaceholderID | String | `"groupPlaceholder"` |
| GroupSeparatorTemplate | ITemplate | `null` |
| GroupTemplate | ITemplate | `null` |
| InsertItemPosition | InsertItemPosition | `None` |
| InsertItemTemplate | ITemplate | `null` |
| InsertMethod | String | `""` |
| ItemPlaceholderID | String | `"itemPlaceholder"` |
| ItemType | String | `""` |
| Page | Page | `null` |
| RenderingCompatibility | Version | `(複合型: Version)` |
| SelectMethod | String | `""` |
| SelectedIndex | Int32 | `-1` |
| SelectedItemTemplate | ITemplate | `null` |
| SelectedPersistedDataKey | DataKey | `null` |
| Site | ISite | `null` |
| TemplateControl | TemplateControl | `null` |
| UpdateMethod | String | `""` |
| ValidateRequestMode | ValidateRequestMode | `Inherit` |

## Literal(対応 3 / 無害 5 / 未対応 7)

| 未対応プロパティ | 型 | 実機の既定値 |
| --- | --- | --- |
| AppRelativeTemplateSourceDirectory | String | `""` |
| Mode | LiteralMode | `Transform` |
| Page | Page | `null` |
| RenderingCompatibility | Version | `(複合型: Version)` |
| Site | ISite | `null` |
| TemplateControl | TemplateControl | `null` |
| ValidateRequestMode | ValidateRequestMode | `Inherit` |

## Panel(対応 16 / 無害 5 / 未対応 11)

| 未対応プロパティ | 型 | 実機の既定値 |
| --- | --- | --- |
| AppRelativeTemplateSourceDirectory | String | `""` |
| BackImageUrl | String | `""` |
| Direction | ContentDirection | `NotSet` |
| HorizontalAlign | HorizontalAlign | `NotSet` |
| Page | Page | `null` |
| RenderingCompatibility | Version | `(複合型: Version)` |
| ScrollBars | ScrollBars | `None` |
| Site | ISite | `null` |
| TemplateControl | TemplateControl | `null` |
| ValidateRequestMode | ValidateRequestMode | `Inherit` |
| Wrap | Boolean | `True` |

## PlaceHolder(対応 2 / 無害 5 / 未対応 6)

| 未対応プロパティ | 型 | 実機の既定値 |
| --- | --- | --- |
| AppRelativeTemplateSourceDirectory | String | `""` |
| Page | Page | `null` |
| RenderingCompatibility | Version | `(複合型: Version)` |
| Site | ISite | `null` |
| TemplateControl | TemplateControl | `null` |
| ValidateRequestMode | ValidateRequestMode | `Inherit` |

## RadioButton(対応 18 / 無害 6 / 未対応 8)

| 未対応プロパティ | 型 | 実機の既定値 |
| --- | --- | --- |
| AppRelativeTemplateSourceDirectory | String | `""` |
| CausesValidation | Boolean | `False` |
| Page | Page | `null` |
| RenderingCompatibility | Version | `(複合型: Version)` |
| Site | ISite | `null` |
| TemplateControl | TemplateControl | `null` |
| ValidateRequestMode | ValidateRequestMode | `Inherit` |
| ValidationGroup | String | `""` |

## RadioButtonList(対応 21 / 無害 7 / 未対応 19)

| 未対応プロパティ | 型 | 実機の既定値 |
| --- | --- | --- |
| AppRelativeTemplateSourceDirectory | String | `""` |
| CausesValidation | Boolean | `False` |
| CellPadding | Int32 | `-1` |
| CellSpacing | Int32 | `-1` |
| DataMember | String | `""` |
| DataSourceID | String | `""` |
| DataTextFormatString | String | `""` |
| ItemType | String | `""` |
| Page | Page | `null` |
| RenderWhenDataEmpty | Boolean | `False` |
| RenderingCompatibility | Version | `(複合型: Version)` |
| RepeatColumns | Int32 | `0` |
| SelectMethod | String | `""` |
| Site | ISite | `null` |
| TemplateControl | TemplateControl | `null` |
| Text | String | `""` |
| TextAlign | TextAlign | `Right` |
| ValidateRequestMode | ValidateRequestMode | `Inherit` |
| ValidationGroup | String | `""` |

## RangeValidator(対応 22 / 無害 7 / 未対応 9)

| 未対応プロパティ | 型 | 実機の既定値 |
| --- | --- | --- |
| AppRelativeTemplateSourceDirectory | String | `""` |
| AssociatedControlID | String | `""` |
| CultureInvariantValues | Boolean | `False` |
| IsValid | Boolean | `True` |
| Page | Page | `null` |
| RenderingCompatibility | Version | `(複合型: Version)` |
| Site | ISite | `null` |
| TemplateControl | TemplateControl | `null` |
| ValidateRequestMode | ValidateRequestMode | `Inherit` |

## RegularExpressionValidator(対応 20 / 無害 7 / 未対応 9)

| 未対応プロパティ | 型 | 実機の既定値 |
| --- | --- | --- |
| AppRelativeTemplateSourceDirectory | String | `""` |
| AssociatedControlID | String | `""` |
| IsValid | Boolean | `True` |
| MatchTimeout | Nullable`1 | `null` |
| Page | Page | `null` |
| RenderingCompatibility | Version | `(複合型: Version)` |
| Site | ISite | `null` |
| TemplateControl | TemplateControl | `null` |
| ValidateRequestMode | ValidateRequestMode | `Inherit` |

## Repeater(対応 8 / 無害 5 / 未対応 10)

| 未対応プロパティ | 型 | 実機の既定値 |
| --- | --- | --- |
| AppRelativeTemplateSourceDirectory | String | `""` |
| DataMember | String | `""` |
| DataSourceID | String | `""` |
| ItemType | String | `""` |
| Page | Page | `null` |
| RenderingCompatibility | Version | `(複合型: Version)` |
| SelectMethod | String | `""` |
| Site | ISite | `null` |
| TemplateControl | TemplateControl | `null` |
| ValidateRequestMode | ValidateRequestMode | `Inherit` |

## RequiredFieldValidator(対応 20 / 無害 7 / 未対応 8)

| 未対応プロパティ | 型 | 実機の既定値 |
| --- | --- | --- |
| AppRelativeTemplateSourceDirectory | String | `""` |
| AssociatedControlID | String | `""` |
| IsValid | Boolean | `True` |
| Page | Page | `null` |
| RenderingCompatibility | Version | `(複合型: Version)` |
| Site | ISite | `null` |
| TemplateControl | TemplateControl | `null` |
| ValidateRequestMode | ValidateRequestMode | `Inherit` |

## TemplateField(対応 3 / 無害 0 / 未対応 13)

| 未対応プロパティ | 型 | 実機の既定値 |
| --- | --- | --- |
| AccessibleHeaderText | String | `""` |
| AlternatingItemTemplate | ITemplate | `null` |
| ConvertEmptyStringToNull | Boolean | `True` |
| EditItemTemplate | ITemplate | `null` |
| FooterTemplate | ITemplate | `null` |
| FooterText | String | `""` |
| HeaderImageUrl | String | `""` |
| HeaderTemplate | ITemplate | `null` |
| InsertItemTemplate | ITemplate | `null` |
| InsertVisible | Boolean | `True` |
| ShowHeader | Boolean | `True` |
| ValidateRequestMode | ValidateRequestMode | `Inherit` |
| Visible | Boolean | `True` |

## TextBox(対応 21 / 無害 7 / 未対応 8)

| 未対応プロパティ | 型 | 実機の既定値 |
| --- | --- | --- |
| AppRelativeTemplateSourceDirectory | String | `""` |
| CausesValidation | Boolean | `False` |
| Page | Page | `null` |
| RenderingCompatibility | Version | `(複合型: Version)` |
| Site | ISite | `null` |
| TemplateControl | TemplateControl | `null` |
| ValidateRequestMode | ValidateRequestMode | `Inherit` |
| ValidationGroup | String | `""` |

## ValidationSummary(対応 19 / 無害 6 / 未対応 8)

| 未対応プロパティ | 型 | 実機の既定値 |
| --- | --- | --- |
| AppRelativeTemplateSourceDirectory | String | `""` |
| Page | Page | `null` |
| RenderingCompatibility | Version | `(複合型: Version)` |
| ShowModelStateErrors | Boolean | `True` |
| ShowValidationErrors | Boolean | `True` |
| Site | ISite | `null` |
| TemplateControl | TemplateControl | `null` |
| ValidateRequestMode | ValidateRequestMode | `Inherit` |

