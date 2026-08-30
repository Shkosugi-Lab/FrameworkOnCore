<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="DefaultsProbe.Default" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>既定レンダリング検証</title>
</head>
<body>
    <form id="form1" runat="server">
        <%-- 対応コントロールを「属性ほぼ未指定」で並べ、WebForms ランタイムの
             既定レンダリングを採取するための適合スイート。
             新しいコントロール/プロパティを変換器に追加したら、必ずここにも足すこと。 --%>

        <h1>既定レンダリング検証</h1>

        <p><asp:Label ID="pLabel" runat="server" Text="ラベル" /></p>
        <p><asp:TextBox ID="pTextBox" runat="server" /></p>
        <p><asp:TextBox ID="pTextArea" runat="server" TextMode="MultiLine" /></p>
        <p><asp:TextBox ID="pPassword" runat="server" TextMode="Password" /></p>
        <p><asp:Button ID="pButton" runat="server" Text="ボタン" CausesValidation="false" /></p>
        <p><asp:LinkButton ID="pLinkButton" runat="server" Text="リンクボタン" CausesValidation="false" /></p>
        <p><asp:HyperLink ID="pHyperLink" runat="server" Text="リンク" NavigateUrl="~/Default.aspx" /></p>
        <p><asp:CheckBox ID="pCheckBox" runat="server" Text="チェック" /></p>
        <asp:Panel ID="pPanel" runat="server">パネル</asp:Panel>

        <p>
            <asp:DropDownList ID="pDropDown" runat="server">
                <asp:ListItem Text="項目A" Value="a" />
                <asp:ListItem Text="項目B" Value="b" />
            </asp:DropDownList>
        </p>

        <asp:RadioButtonList ID="pRadioV" runat="server">
            <asp:ListItem Text="縦A" Value="a" />
            <asp:ListItem Text="縦B" Value="b" />
        </asp:RadioButtonList>

        <asp:RadioButtonList ID="pRadioH" runat="server" RepeatDirection="Horizontal">
            <asp:ListItem Text="横A" Value="a" />
            <asp:ListItem Text="横B" Value="b" />
        </asp:RadioButtonList>

        <p><asp:HiddenField ID="pHidden" runat="server" Value="hidden-value" /></p>
        <p><asp:Image ID="pImage" runat="server" ImageUrl="~/probe.png" /></p>
        <p><asp:Image ID="pImageAlt" runat="server" ImageUrl="~/probe.png" AlternateText="代替テキスト" /></p>

        <p>
            <asp:RadioButton ID="pRadioBtn1" runat="server" GroupName="pg" Text="単体A" />
            <asp:RadioButton ID="pRadioBtn2" runat="server" GroupName="pg" Text="単体B" Checked="true" />
        </p>

        <asp:CheckBoxList ID="pCheckList" runat="server">
            <asp:ListItem Text="選甲" Value="1" />
            <asp:ListItem Text="選乙" Value="2" Selected="True" />
            <asp:ListItem Text="選丙" Value="3" />
        </asp:CheckBoxList>

        <p><asp:FileUpload ID="pFile" runat="server" /></p>

        <asp:DataList ID="pDataListV" runat="server">
            <%-- DataBinder.Eval 長形式(DNN 世代の定番記法)の変換検証 --%>
            <ItemTemplate><span><%# DataBinder.Eval(Container.DataItem, "Name") %></span></ItemTemplate>
        </asp:DataList>

        <asp:DataList ID="pDataListH" runat="server" RepeatColumns="2" RepeatDirection="Horizontal">
            <ItemTemplate><span><%# Eval("Name") %></span></ItemTemplate>
        </asp:DataList>

        <%-- expando 属性(サーバープロパティに一致しない属性はそのまま描画される) --%>
        <p><asp:TextBox ID="pExpando" runat="server" autocomplete="off" data-probe="x" /></p>
        <asp:Panel ID="pClassPanel" runat="server" class="raw-class">クラス素通し</asp:Panel>
        <p><asp:Button ID="pStyleBtn" runat="server" Text="スタイル" Style="margin-left:5px" CausesValidation="false" /></p>

        <%-- 式ビルダー(AppSettings)とインライン if --%>
        <p><asp:Label ID="pExprSetting" runat="server" Text="<%$ AppSettings:ProbeSetting %>" /></p>

        <% if (ShowExtraSection) { %>
            <div id="pIfShown">条件付き表示(true 側)</div>
        <% } else { %>
            <div id="pIfHidden">条件付き表示(false 側)</div>
        <% } %>

        <%-- Pack 1-3: VerticalAlign / PagerStyle / 画像リンク / CheckBoxList 列 / 宣言的データソース --%>
        <asp:DataList ID="pDataListVA" runat="server" ItemStyle-VerticalAlign="Top">
            <ItemTemplate><span><%# Eval("Name") %></span></ItemTemplate>
        </asp:DataList>

        <asp:GridView ID="pGridPager" runat="server" AllowPaging="true" PageSize="2"
            PagerStyle-CssClass="pager-style" />

        <p><asp:HyperLink ID="pHlImage" runat="server" NavigateUrl="~/Default.aspx"
            ImageUrl="~/probe.png" Text="画像リンク" /></p>

        <asp:CheckBoxList ID="pCheckCols" runat="server" RepeatColumns="2" RepeatDirection="Horizontal">
            <asp:ListItem Text="列甲" Value="1" />
            <asp:ListItem Text="列乙" Value="2" />
            <asp:ListItem Text="列丙" Value="3" />
        </asp:CheckBoxList>

        <asp:ObjectDataSource ID="odsProbe" runat="server" TypeName="DefaultsProbe.ProbeData"
            SelectMethod="GetItems" />
        <asp:GridView ID="pGridOds" runat="server" DataSourceID="odsProbe" />

        <%-- DataTable バインド(IListSource / DataRowView)の適合検証 --%>
        <asp:GridView ID="pGridDt" runat="server" />

        <%-- DataGrid(GridView の前身)と <%= の非エンコード描画 --%>
        <asp:DataGrid ID="pDataGrid" runat="server" AutoGenerateColumns="false">
            <Columns>
                <asp:BoundColumn DataField="Name" HeaderText="名前" />
                <asp:BoundColumn DataField="Price" HeaderText="価格" DataFormatString="{0:N0}" />
            </Columns>
        </asp:DataGrid>
        <p id="pRawExpr"><%= RawHtmlFragment %></p>

        <%-- Render(HtmlTextWriter) 型カスタムコントロール(LegacyRenderHost 検証) --%>
        <p><probe:FancyBadge ID="pBadge" runat="server" Label="バッジ" Count="3" /></p>

        <probe:FancyPanel ID="pFancyPanel" runat="server" CssClass="fp">
            中身テキスト <asp:Label ID="pInnerLabel" runat="server" Text="内側ラベル" />
        </probe:FancyPanel>

        <%-- モデルバインディング(ItemType / SelectMethod / 型付き Item 参照) --%>
        <asp:ListView ID="pModelList" runat="server" ItemType="DefaultsProbe.ProbeItem" SelectMethod="GetModelItems">
            <LayoutTemplate>
                <ul id="model-list"><li id="itemPlaceholder" runat="server"></li></ul>
            </LayoutTemplate>
            <ItemTemplate>
                <li><%#: Item.Name %>=<%#: Item.Price %></li>
            </ItemTemplate>
        </asp:ListView>

        <%-- HtmlControls(runat="server" の素の HTML)— タグ保持の適合検証 --%>
        <div runat="server" id="pServerDiv" class="sd">
            サーバーDIV <asp:Label ID="pDivLabel" runat="server" Text="div内" />
        </div>
        <table>
            <tr runat="server" id="pServerRow"><td>サーバー行</td></tr>
        </table>

        <%-- Server comment whose body contains %> : text after it must survive --%>
        <p id="pAfterComment"><%-- 隠し断片 <%# Eval("Hidden") %> --%>コメント後も表示</p>

        <%-- App_GlobalResources の式ビルダー(ASP.NET 標準のローカライズ) --%>
        <p><asp:Label ID="pResource" runat="server" Text="<%$ Resources: ProbeStrings, ProbeGreeting %>" /></p>

        <%-- タグを跨ぐ <% if %>(決定的変換では残差になる)。
             ShowWrapper=false 側を検証することで、ラッパー div が出ないことを確認する --%>
        <% if (ShowWrapper)
           { %>
        <div id="pWrapOuter" class="wrapouter">
            <div id="pWrapInner">
                <% } %>
                <asp:Label ID="pWrapped" runat="server" Text="ラッパー内容" />
                <% if (ShowWrapper)
                   { %>
            </div>
        </div>
        <% } %>

        <%-- 標準コントロール追加分(ImageButton / ListBox / Table 系)と表示系基底継承 --%>
        <p><asp:ImageButton ID="pImgBtn" runat="server" ImageUrl="~/probe.png" AlternateText="画像ボタン" CausesValidation="false" /></p>

        <asp:ListBox ID="pListBox" runat="server" Rows="3">
            <asp:ListItem Text="LB甲" Value="1" />
            <asp:ListItem Text="LB乙" Value="2" Selected="True" />
        </asp:ListBox>

        <asp:Table ID="pTable" runat="server">
            <asp:TableRow runat="server">
                <asp:TableHeaderCell runat="server">見出しセル</asp:TableHeaderCell>
                <asp:TableCell runat="server">内容セル</asp:TableCell>
            </asp:TableRow>
        </asp:Table>

        <p><probe:FancyLink ID="pFancyLink" runat="server" NavigateUrl="~/Default.aspx" Text="継承リンク" /></p>

        <asp:DetailsView ID="pDetails" runat="server" AutoGenerateRows="false">
            <Fields>
                <asp:BoundField DataField="Name" HeaderText="名前" />
                <asp:BoundField DataField="Price" HeaderText="価格" DataFormatString="{0:N0}" />
            </Fields>
        </asp:DetailsView>

        <%-- ItemDataBound / RowDataBound / RowDeleting --%>
        <asp:DataList ID="pDataListEvt" runat="server" OnItemDataBound="pDataListEvt_ItemDataBound">
            <ItemTemplate><asp:HyperLink ID="hlItem" runat="server" />
                <asp:TextBox ID="txtEvtVal" runat="server" Value='<%# Eval("Name") %>' /></ItemTemplate>
        </asp:DataList>

        <asp:GridView ID="pGridEvt" runat="server" AutoGenerateColumns="false" DataKeyNames="Name"
            OnRowDataBound="pGridEvt_RowDataBound" OnRowDeleting="pGridEvt_RowDeleting">
            <Columns>
                <asp:BoundField DataField="Name" HeaderText="名前" />
                <asp:TemplateField HeaderText="操作">
                    <ItemTemplate>
                        <asp:Label ID="lblEvt" runat="server" />
                        <asp:LinkButton ID="lnkDel" runat="server" CommandName="Delete" Text="削除" CausesValidation="false" />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>

        <asp:GridView ID="pGridAuto" runat="server" />

        <asp:GridView ID="pGridColumns" runat="server" AutoGenerateColumns="false">
            <Columns>
                <asp:BoundField DataField="Name" HeaderText="名前" />
                <asp:BoundField DataField="Price" HeaderText="価格" DataFormatString="{0:N0}" />
            </Columns>
        </asp:GridView>

        <asp:FormView ID="pFormView" runat="server">
            <ItemTemplate>
                <span><%# Eval("Name") %></span>
            </ItemTemplate>
        </asp:FormView>

        <p>
            <asp:TextBox ID="pReqTarget" runat="server" />
            <asp:RequiredFieldValidator ID="pRequired" runat="server" ControlToValidate="pReqTarget"
                ErrorMessage="必須エラーの既定表示" />
        </p>
        <p>
            <asp:TextBox ID="pRangeTarget" runat="server" Text="99" />
            <asp:RangeValidator ID="pRange" runat="server" ControlToValidate="pRangeTarget"
                Type="Integer" MinimumValue="1" MaximumValue="5" ErrorMessage="範囲エラーの既定表示" />
        </p>
        <asp:ValidationSummary ID="pSummary" runat="server" />
        <p><asp:Button ID="btnValidate" runat="server" Text="検証実行" /></p>

        <%-- === 明示指定の描画(頻出プロパティの適合検証) === --%>
        <h2>明示指定の描画</h2>

        <asp:GridView ID="pGridStyled" runat="server" AutoGenerateColumns="false"
            Caption="スタイル付きグリッド" DataKeyNames="Name">
            <HeaderStyle CssClass="head-style" />
            <RowStyle CssClass="row-style" BackColor="#EEEEEE" />
            <AlternatingRowStyle CssClass="alt-style" />
            <Columns>
                <asp:BoundField DataField="Name" HeaderText="名前" />
                <asp:BoundField DataField="Price" HeaderText="価格" DataFormatString="{0:N0}" />
            </Columns>
        </asp:GridView>

        <asp:GridView ID="pGridFieldStyles" runat="server" AutoGenerateColumns="false">
            <Columns>
                <asp:BoundField DataField="Name" HeaderText="名前"
                    ItemStyle-Width="120" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Right" />
                <asp:BoundField DataField="Price" HeaderText="価格" DataFormatString="{0:N0}"
                    HeaderStyle-Width="80" ItemStyle-HorizontalAlign="Right" />
            </Columns>
        </asp:GridView>

        <asp:Panel ID="pFieldset" runat="server" GroupingText="グループ見出し" DefaultButton="btnValidate">
            <asp:Label ID="pLabelFor" runat="server" Text="関連ラベル" AssociatedControlID="pTextBox" />
        </asp:Panel>

        <%-- 小文字の style は WebForms マークアップでの一般形。サーバープロパティ
             (Style コレクション)と同名なので expando 判定では拾えず、描画へ通す
             専用の経路が要る。落とすと見た目が黙って変わる。 --%>
        <p><asp:Label ID="pStyleLower" runat="server" Text="小文字style" style="color:#c00;font-weight:bold" /></p>
        <p><asp:Panel ID="pStyleUpper" runat="server" Style="border:1px solid #00c">大文字Style</asp:Panel></p>

        <%-- 暗黙ローカライズ: App_LocalResources の "pLocalized.Text" / ".ToolTip" が
             マークアップの指定を上書きする。WebForms は実質コンパイル時に決めるので、
             変換器も静的に解決する。 --%>
        <p><asp:Label ID="pLocalized" runat="server" meta:resourcekey="pLocalized" Text="上書き前" ToolTip="上書き前" /></p>

        <p><asp:TextBox ID="pNoWrap" runat="server" TextMode="MultiLine" Wrap="false" /></p>
        <p><asp:CheckBox ID="pCheckLeft" runat="server" Text="左ラベル" TextAlign="Left" /></p>
        <p>
            <asp:DropDownList ID="pAppend" runat="server" AppendDataBoundItems="true"
                DataTextField="Name" DataValueField="Name">
                <asp:ListItem Text="(選択してください)" Value="" />
            </asp:DropDownList>
        </p>
        <p>
            <asp:TextBox ID="pDynTarget" runat="server" />
            <asp:RequiredFieldValidator ID="pRequiredDyn" runat="server" ControlToValidate="pDynTarget"
                Display="Dynamic" ErrorMessage="Dynamic 表示の必須エラー" />
        </p>
        <asp:ValidationSummary ID="pSummaryList" runat="server" HeaderText="入力エラー:" DisplayMode="List" />
    </form>
</body>
</html>
