# Writes corpora/regression/wt.operations.json (the inventory ParityTest reads) and wt.operations.md (the same, to read)
# from the table below: every user-facing operation of WingtipToys, found by reading the original's source (pages,
# controls, code-behind, web.config, Global.asax, Startup.Auth) and by crawling the running application (ParityTest
# discover: corpora/regression/wt.discovered.*.json). Edit the table and run: python corpora/regression/wt-operations-inventory.py
import json, os

A, U, ADM, ANY = 'anonymous', 'user', 'admin (canEdit)', 'any'
OPS = []


def op(id, category, page, element, expected, role=ANY, pre=None, inputs=None, keys=None, runnable=True, reason=None, partial=None):
    OPS.append({k: v for k, v in dict(id=id, category=category, page=page, element=element, preconditions=pre, role=role,
                                       inputs=inputs, expected=expected, keys=keys, runnable=runnable, reason=reason,
                                       partial=partial).items() if v is not None})


# --- master page (Site.Master) ---
op('master.anonymous', 'master', 'Site.Master', 'LoginView AnonymousTemplate: Register / Log in links', 'links to /Account/Register, /Account/Login', A,
   keys=['GET /account/register', 'GET /account/login'])
op('master.authenticated', 'master', 'Site.Master', 'LoginView LoggedInTemplate: "Hello, <user> !" (link to Manage)', 'greeting with the user name, link /Account/Manage', U,
   keys=['GET /account/manage'])
op('master.categories', 'master', 'Site.Master', 'ListView categoryList (SelectMethod GetCategories): a link per category (route ProductsByCategoryRoute)', '5 links /Category/<name>, separated by |',
   keys=['GET /category/*'])
op('master.cart-count', 'master', 'Site.Master', 'cartCount link: "Cart (n)" (Page_PreRender, ShoppingCartActions.GetCount)', 'the sum of the quantities of the session\'s cart',
   keys=['GET /shoppingcart'])
op('master.menu-links', 'master', 'Site.Master', 'navbar links Home / About / Contact / Products / brand', 'each page',
   keys=['GET /', 'GET /about', 'GET /contact', 'GET /productlist'])
op('master.logo-link', 'master', 'Site.Master', 'logo image (Image1) linking to ~/', 'the home page', keys=['GET /'])
op('master.admin-link', 'master', 'Site.Master', 'adminLink (visible for the canEdit role)', 'the Admin link only for canEdit', ADM,
   keys=['GET /admin/adminpage'])
op('acct.logoff', 'account', 'Site.Master', 'LoginStatus "Log off" (__doPostBack, OnLoggingOut: OWIN SignOut, LogoutPageUrl ~/)', 'signed out, redirect to /, the authentication cookie removed', U,
   keys=['POSTBACK * ctl00$ctl00', 'POSTBACK * ctl00$ctl*'])

# --- navigation ---
op('nav.home', 'page', '/ (Default.aspx)', 'GET', 'the home page', keys=['GET /', 'GET /default'])
op('nav.aspx-redirect', 'page', 'FriendlyUrls', 'GET of a page by its .aspx name (AutoRedirectMode Permanent)', '301 to the extensionless URL, the query kept',
   keys=['GET /*.aspx*'])
op('nav.about', 'page', '/About', 'GET', 'the page')
op('nav.contact', 'page', '/Contact', 'GET (mailto links)', 'the page', keys=['EXTERNAL mailto:*'])
op('nav.products', 'page', '/ProductList', 'GET: ListView productList (SelectMethod GetProducts, GroupItemCount 4)', '16 products, 4 a row, prices in the server culture\'s currency')
op('nav.products-by-id', 'page', '/ProductList?id=', 'QueryString model binding [QueryString("id")] int? categoryId', 'the category\'s products', inputs=['id=2'],
   keys=['GET /productlist?id'])
op('nav.products-by-id-invalid', 'page', '/ProductList?id=abc', 'model binding of an unparsable int?', 'null: every product', inputs=['id=abc'])
op('nav.products-empty', 'page', '/ProductList?id=99, /Category/Nothing', 'ListView EmptyDataTemplate', '"No data was returned."', inputs=['id=99', 'categoryName=Nothing'])
op('nav.category-route', 'page', '/Category/{categoryName}', 'route ProductsByCategoryRoute, [RouteData] categoryName', 'the category\'s products', inputs=['Planes', 'Rockets'],
   keys=['GET /category/*'])
op('nav.product-route', 'page', '/Product/{productName}', 'route ProductByNameRoute (escaped space: Fast%20Car), FormView productDetail', 'the product\'s details',
   keys=['GET /product/*'])
op('nav.product-by-id', 'page', '/ProductDetails?ProductID=', '[QueryString("ProductID")] int? productId', 'the product\'s details', keys=['GET /productdetails?productid'])
op('nav.product-none', 'page', '/ProductDetails', 'no / unknown / unparsable ProductID, unknown name', 'the FormView renders nothing', inputs=['(none)', 'ProductID=999', 'ProductID=abc', '/Product/Nothing'],
   keys=['GET /productdetails'])
op('nav.mobile-switch', 'page', '/__FriendlyUrls_SwitchView/{view}', 'FriendlyUrls\' view switch (cookie), ViewSwitcher.ascx on Site.Mobile.Master', 'the mobile master page; "Switch to Desktop" back',
   keys=['GET /__friendlyurls_switchview/*'])
op('nav.mobile-browser', 'page', 'any page', 'a phone\'s browser (User-Agent: iPhone): Request.Browser.IsMobileDevice', 'Site.Mobile.Master chosen by FriendlyUrls')
op('nav.not-found-extensionless', 'error', '/NoSuchPage', 'GET of a URL with no page', 'IIS 404.0 (detailed, local)')
op('nav.not-found-aspx', 'error', '/NoSuch.aspx', 'GET of a missing .aspx', 'ASP.NET 404 (HttpException, customErrors Off)')
op('nav.elmah', 'page', '/elmah.axd', 'ELMAH\'s error log (handler; allowRemoteAccess false)', 'the run\'s errors, newest first; its style sheet and RSS', ADM)

# --- static content ---
op('static.page-assets', 'static', 'every page', 'the page\'s scripts, style sheets, icon, images (bundles /bundles/*, /Content/css, ScriptManager\'s scripts)', 'each one 200 with the same content',
   keys=['GET /bundles/*', 'GET /content/*', 'GET /scripts/*'])
op('static.files', 'static', '/Content, /Images, /Catalog/Images, /Scripts, /fonts, favicon', 'static files', '200, the same bytes and type',
   keys=['GET /catalog/images/*', 'GET /images/*', 'GET /favicon.ico'])
op('static.not-found', 'static', '/Content/missing.css', 'a missing static file', '404')
op('static.request-filtering', 'static', 'Web.config, packages.config, bin, App_Data, Global.asax, .master, .cs, .ascx', 'files the server refuses', 'IIS 404.8 / 404.7 / ASP.NET 403 (HttpForbiddenHandler) as the original')

# --- error page ---
op('err.error-page', 'error', '/ErrorPage', 'ErrorPage.aspx without an error', 'the generic message; details for a local request')
op('err.error-page-404', 'error', '/ErrorPage?msg=404', 'msg=404', '"An HTTP error occurred. Page Not found."', inputs=['msg=404'], keys=['GET /errorpage?msg'])
op('err.error-page-handler', 'error', '/ErrorPage?handler=', 'handler shown in the details', 'the handler text (local request)', inputs=['handler=Parity handler'], keys=['GET /errorpage?handler'])

# --- cart ---
op('cart.empty', 'cart', '/ShoppingCart', 'an empty cart', '"Shopping Cart is Empty", no Update / checkout buttons')
op('cart.add', 'cart', '/AddToCart.aspx?productID=', '"Add To Cart" link of the product list (AddToCart.Page_Load, Response.Redirect ShoppingCart.aspx)', 'the cart with the product, quantity 1; Cart (n) in the master',
   keys=['GET /addtocart?productid'])
op('cart.add-again', 'cart', '/AddToCart?ProductID=', 'the same product again', 'its quantity + 1')
op('cart.session-guid', 'cart', 'ShoppingCartActions.GetCartId', 'an anonymous cart\'s id (a GUID in the session)', 'CartItems rows under the session\'s GUID')
op('cart.session-isolation', 'cart', '/ShoppingCart', 'another browser (session)', 'its own, empty cart')
op('cart.add-invalid', 'cart', '/AddToCart', 'no / unparsable / out-of-Int16 ProductID', '500: the application\'s exception / OverflowException', inputs=['(none)', 'abc', '70000'])
op('cart.add-unknown-product', 'cart', '/AddToCart?ProductID=999', 'a product that does not exist', '500: the FOREIGN KEY violation (DbUpdateException)')
op('cart.update-quantity', 'cart', '/ShoppingCart', 'GridView CartList: TextBox PurchaseQuantity + Button UpdateBtn (UpdateBtn_Click, GetValues/ExtractValuesFromCell)', 'the quantity and the totals updated', inputs=['3'],
   keys=['INPUT /shoppingcart ctl00$MainContent$CartList$ctl*$PurchaseQuantity text', 'SUBMIT /shoppingcart ctl00$MainContent$UpdateBtn'])
op('cart.update-zero', 'cart', '/ShoppingCart', 'quantity 0', 'the row removed', inputs=['0'])
op('cart.update-negative', 'cart', '/ShoppingCart', 'quantity -1', 'the row removed', inputs=['-1'])
op('cart.update-invalid', 'cart', '/ShoppingCart', 'quantity not a number', '500 FormatException (Convert.ToInt16)', inputs=['abc'])
op('cart.update-overflow', 'cart', '/ShoppingCart', 'quantity out of Int16', '500 OverflowException', inputs=['99999'])
op('cart.remove-checkbox', 'cart', '/ShoppingCart', 'CheckBox Remove + Update', 'the row removed', keys=['INPUT /shoppingcart ctl00$MainContent$CartList$ctl*$Remove checkbox'])
op('cart.checkout-anonymous', 'cart', '/ShoppingCart', 'ImageButton CheckoutImageBtn (PayPal) as an anonymous user', 'Session["payment_amt"] set, redirect to Checkout/CheckoutStart.aspx, which sends to the login page (ReturnUrl)', A,
   keys=['SUBMIT /shoppingcart ctl00$MainContent$CheckoutImageBtn'])
op('cart.checkout', 'cart', '/ShoppingCart', 'ImageButton CheckoutImageBtn logged in', 'redirect to /Checkout/CheckoutStart (PayPal)', U)
op('cart.migrate-on-login', 'cart', '/Account/Login', 'ShoppingCartActions.MigrateCart(cartId, Email) on login', 'the anonymous cart\'s rows under the user name; the session\'s cart id the user name', U)
op('cart.migrate-on-register', 'cart', '/Account/Register', 'MigrateCart(cartId, user.Id) on register', 'the rows under the new user\'s Id', U)

# --- checkout ---
op('checkout.paypal-start', 'checkout', '/Checkout/CheckoutStart', 'NVPAPICaller.ShortcutExpressCheckout: a real call to PayPal\'s sandbox NVP API (HttpWebRequest)', 'PayPal answers ACK=Failure 10002 (the sample\'s placeholder credentials): redirect to CheckoutError?ErrorCode=10002&Desc=Security error&Desc2=Security header is not valid; no order', U,
   partial='as far as the original goes without PayPal credentials: the API call and its error redirect; no PayPal page, no token')
op('checkout.start-no-amount', 'checkout', '/Checkout/CheckoutStart', 'without Session["payment_amt"]', 'redirect CheckoutError?ErrorCode=AmtMissing', U)
op('checkout.review-no-token', 'checkout', '/Checkout/CheckoutReview', 'without Session["token"] (no PayPal return)', '500 NullReferenceException', U)
op('checkout.complete-unvalidated', 'checkout', '/Checkout/CheckoutComplete', 'without Session["userCheckoutCompleted"]', 'redirect CheckoutError?Desc=Unvalidated Checkout.', U)
op('checkout.cancel', 'checkout', '/Checkout/CheckoutCancel', 'GET (PayPal\'s cancel URL)', 'the page', U)
op('checkout.error-page', 'checkout', '/Checkout/CheckoutError', 'ErrorCode / Desc / Desc2 written raw (<%= %>)', 'the three values', U, inputs=['ErrorCode=E1&Desc=..&Desc2=..'],
   keys=['GET /checkout/checkouterror*'])
op('checkout.paypal-return', 'checkout', '/Checkout/CheckoutReview', 'PayPal\'s return (GetExpressCheckoutDetails): the order saved, OrderItemList / ShipInfo shown', 'the order review', U,
   runnable=False, reason='needs a PayPal sandbox account and a buyer\'s approval on PayPal\'s site: the sample has placeholder API credentials, so PayPal never issues a token (the call is exercised up to its error, checkout.paypal-start)')
op('checkout.review-confirm', 'checkout', '/Checkout/CheckoutReview', 'Button CheckoutConfirm "Complete Order"', 'redirect CheckoutComplete', U,
   runnable=False, reason='the button is only rendered by a successful PayPal return (checkout.paypal-return)')
op('checkout.complete-payment', 'checkout', '/Checkout/CheckoutComplete', 'DoExpressCheckoutPayment: the transaction id saved, the cart emptied', 'Checkout Complete with the transaction id', U,
   runnable=False, reason='needs the PayPal token and payer id of checkout.paypal-return; its guard (no validated checkout) is exercised: checkout.complete-unvalidated')
op('checkout.complete-continue', 'checkout', '/Checkout/CheckoutComplete', 'Button Continue "Continue Shopping"', 'redirect ~/Default.aspx', U,
   runnable=False, reason='the page only renders its button after a completed payment (checkout.complete-payment)')

# --- authorization ---
op('authz.manage', 'authorization', '/Account/Manage', 'Account/Web.config <location path="Manage.aspx"><deny users="?"/> (also through its route)', '302 to /Account/Login?ReturnUrl=', A)
op('authz.checkout', 'authorization', '/Checkout/*', 'Checkout/Web.config <deny users="?"/>', '302 to the login page', A)
op('authz.admin-anonymous', 'authorization', '/Admin/*', 'Admin/Web.config <allow roles="canEdit"/><deny users="*"/> (any case of the URL)', '302 to the login page', A)
op('authz.admin-user', 'authorization', '/Admin/AdminPage', 'a logged-in user who is not in canEdit', '401 turned into a redirect to the login page by OWIN\'s cookie middleware', U)
op('authz.anonymous-account-pages', 'authorization', '/Account/ManagePassword, ManageLogins, VerifyPhoneNumber, AddPhoneNumber (submit)', 'pages not protected by web.config, used anonymously', '500 InvalidOperationException "UserId not found."', A)

# --- account ---
op('acct.register-page', 'account', '/Account/Register', 'GET (with the ReturnUrl of the login page\'s link)', 'the form', A, keys=['GET /account/register?returnurl'])
op('acct.login-register-link', 'account', '/Account/Login', 'HyperLink RegisterHyperLink (ReturnUrl passed on)', '/Account/Register?ReturnUrl=...', A)
op('acct.register-required', 'account', '/Account/Register', 'RequiredFieldValidators (Email, Password, ConfirmPassword) + ValidationSummary: client side, and server side (client validation off)', 'the messages', A,
   inputs=['(empty)'], keys=['VALIDATOR /account/register * RequiredFieldValidatorEvaluateIsValid', 'INPUT /account/register *', 'SUBMIT /account/register ctl00$MainContent$ctl*'])
op('acct.register-mismatch', 'account', '/Account/Register', 'CompareValidator ConfirmPassword', '"The password and confirmation password do not match."', A, keys=['VALIDATOR /account/register * CompareValidatorEvaluateIsValid'])
op('acct.register-weak-password', 'account', '/Account/Register', 'Identity PasswordValidator (6+, non-alphanumeric, digit, lower, upper)', 'Identity\'s message', A, inputs=['abc', 'abcdefgh'])
op('acct.register-invalid-email', 'account', '/Account/Register', 'TextMode=Email (the browser refuses), and the server\'s UserValidator (RequireUniqueEmail)', 'browser: not sent; server: "Email \'notanemail\' is invalid."', A, inputs=['notanemail'])
op('acct.register', 'account', '/Account/Register', 'Button "Register" (CreateUser_Click): UserManager.Create, SignIn, RedirectToReturnUrl', 'signed in, redirect to the ReturnUrl; AspNetUsers row', A)
op('acct.register-duplicate', 'account', '/Account/Register', 'an existing user name', '"Name ... is already taken."', ANY)
op('acct.login-page', 'account', '/Account/Login', 'GET', 'the form, Google\'s button (OpenAuthProviders)', A, keys=['GET /account/login?returnurl'])
op('acct.login-required', 'account', '/Account/Login', 'RequiredFieldValidators Email, Password (client, server)', 'the messages', A,
   keys=['VALIDATOR /account/login * RequiredFieldValidatorEvaluateIsValid', 'INPUT /account/login *', 'SUBMIT /account/login ctl00$MainContent$ctl*'])
op('acct.login-wrong-password', 'account', '/Account/Login', 'PasswordSignIn failure', '"Invalid login attempt"', A)
op('acct.login-unknown-user', 'account', '/Account/Login', 'an unknown user', '"Invalid login attempt"', A)
op('acct.login', 'account', '/Account/Login', 'Button "Log in" (LogIn): PasswordSignIn success', 'signed in, redirect to ReturnUrl or ~/', A)
op('acct.login-return-url', 'account', '/Account/Login?ReturnUrl=/About', 'IdentityHelper.RedirectToReturnUrl (a local URL)', 'redirect to /About', A)
op('acct.login-return-url-external', 'account', '/Account/Login?ReturnUrl=//evil.example/', 'IdentityHelper.IsLocalUrl refuses //host', 'redirect to ~/', A)
op('acct.login-remember-me', 'account', '/Account/Login', 'CheckBox RememberMe', 'a persistent authentication cookie', A, keys=['INPUT /account/login ctl00$MainContent$RememberMe checkbox'])
op('acct.login-locked-out', 'account', '/Account/Login', 'SignInStatus.LockedOut', 'redirect /Account/Lockout', A, pre='the user locked out (acct.two-factor-lockout)')
op('acct.lockout-page', 'account', '/Account/Lockout', 'GET', 'the page')
op('acct.manage', 'account', '/Account/Manage', "GET: [Change] password, External Logins count, [Manage], the two-factor article's link", 'the page', U,
   keys=['EXTERNAL http://go.microsoft.com/fwlink/*'])
op('acct.manage-messages', 'account', '/Account/Manage?m=', 'm = ChangePwdSuccess / SetPwdSuccess / RemoveLoginSuccess / AddPhoneNumberSuccess / RemovePhoneNumberSuccess / other (Form.Action stripped)', 'each message', U,
   keys=['GET /account/manage?m'])
op('acct.manage-no-password', 'account', '/Account/Manage', 'a user without a password', '[Create] instead of [Change]', U, pre='PasswordHash NULL (set in the database)')
op('acct.change-password-page', 'account', '/Account/ManagePassword', 'GET with a password: Change Password form', 'the form', U, keys=['GET /account/managepassword'])
op('acct.change-password-required', 'account', '/Account/ManagePassword', 'RequiredFieldValidators of ValidationGroup ChangePassword (client, server)', 'the messages', U,
   keys=['VALIDATOR /account/managepassword *', 'INPUT /account/managepassword *', 'SUBMIT /account/managepassword ctl00$MainContent$ctl*'])
op('acct.change-password-mismatch', 'account', '/Account/ManagePassword', 'CompareValidator ConfirmNewPassword', 'the message', U)
op('acct.change-password-wrong-current', 'account', '/Account/ManagePassword', 'ChangePassword failure: ModelState error in the ValidationSummary (ShowModelStateErrors)', '"Incorrect password."', U)
op('acct.change-password-weak', 'account', '/Account/ManagePassword', 'PasswordValidator', 'Identity\'s message', U)
op('acct.change-password', 'account', '/Account/ManagePassword', 'Button "Change Password": ChangePassword, SignIn', 'redirect Manage?m=ChangePwdSuccess; the old password refused, the new one accepted', U)
op('acct.set-password', 'account', '/Account/ManagePassword', 'Set Password form (no password): AddPassword', 'redirect Manage?m=SetPwdSuccess', U, pre='PasswordHash NULL (set in the database: only an external-login account has none)')
op('acct.set-password-required', 'account', '/Account/ManagePassword', 'RequiredFieldValidators of ValidationGroup SetPassword; ModelErrorMessage NewPassword', 'the messages', U)
op('acct.set-password-mismatch', 'account', '/Account/ManagePassword', 'CompareValidator ControlToCompare="Password" (the TextBox is "password": FindControl ignores case)', 'the message', U)
op('acct.set-password-weak', 'account', '/Account/ManagePassword', 'PasswordValidator in AddPassword', 'Identity\'s message', U)
op('acct.manage-logins', 'account', '/Account/ManageLogins', 'ListView (SelectMethod GetLogins) without logins; OpenAuthProviders (ReturnUrl ~/Account/ManageLogins)', 'no rows; Google\'s button', U, keys=['GET /account/managelogins'])
op('acct.manage-logins-remove', 'account', '/Account/ManageLogins', 'Button "Remove" (CommandName Delete, DeleteMethod RemoveLogin with the DataKeys LoginProvider, ProviderKey)', 'the login removed, redirect ?m=RemoveLoginSuccess', U, pre='an AspNetUserLogins row (set in the database)',
   keys=['GET /account/managelogins?m'])
op('acct.external-login', 'account', '/Account/Login', '<button name="provider" value="Google">: OWIN Challenge (Google OAuth 2), Response.End', '302 to accounts.google.com (client id, redirect_uri /signin-google, scope, state); correlation cookie', A,
   keys=['SUBMIT /account/login provider=Google'])
op('acct.external-login-link', 'account', '/Account/ManageLogins', 'the same button signed in (XsrfId in the properties)', '302 to Google', U, keys=['SUBMIT /account/managelogins provider=Google'])
op('acct.external-callback', 'account', '/signin-google', 'Katana\'s Google middleware callback without Google\'s answer', '500 (no state)', A)
op('acct.register-external', 'account', '/Account/RegisterExternalLogin', 'without a provider / without an external login cookie', 'redirect to the login page', A, keys=['GET /account/registerexternallogin*'])
op('acct.register-external-create', 'account', '/Account/RegisterExternalLogin', 'Button "Log in" (LogIn_Click): a user created from the external login', '', A,
   runnable=False, reason='needs Google\'s answer (an external login cookie): the redirect to Google and the callback are exercised (acct.external-login, acct.external-callback)')
op('acct.external-callback-success', 'account', '/signin-google', 'Google\'s answer: the external cookie, RegisterExternalLogin', '', A,
   runnable=False, reason='needs a Google client id and a Google account (the sample has placeholder ids)')
op('acct.add-phone-page', 'account', '/Account/AddPhoneNumber', 'GET', 'the form', U, keys=['GET /account/addphonenumber'])
op('acct.add-phone-required', 'account', '/Account/AddPhoneNumber', 'RequiredFieldValidator PhoneNumber', 'the message', U, keys=['VALIDATOR /account/addphonenumber *', 'INPUT /account/addphonenumber *', 'SUBMIT /account/addphonenumber *'])
op('acct.add-phone', 'account', '/Account/AddPhoneNumber', 'Button "Submit": GenerateChangePhoneNumberToken, SmsService (sends nothing)', 'redirect VerifyPhoneNumber?PhoneNumber=', U, keys=['GET /account/verifyphonenumber?phonenumber'])
op('acct.verify-phone-required', 'account', '/Account/VerifyPhoneNumber', 'RequiredFieldValidator Code', 'the message', U, keys=['VALIDATOR /account/verifyphonenumber *', 'INPUT /account/verifyphonenumber *', 'SUBMIT /account/verifyphonenumber *'])
op('acct.verify-phone-wrong-code', 'account', '/Account/VerifyPhoneNumber', 'ChangePhoneNumber failure', '"Failed to verify phone" in the summary', U)
op('acct.verify-phone', 'account', '/Account/VerifyPhoneNumber', 'the right code (the TOTP of the user\'s security stamp and the number)', 'redirect Manage?m=AddPhoneNumberSuccess; PhoneNumber saved', U)
op('acct.manage-remove-phone', 'account', '/Account/Manage', 'LinkButton [Remove] (RemovePhone_Click)', '', U, runnable=False,
   reason='not in the user interface: the markup of the phone number\'s links is commented out (<%-- --%>), so nothing raises the handler')
op('acct.manage-two-factor-toggle', 'account', '/Account/Manage', 'LinkButtons [Enable] / [Disable] (TwoFactorEnable_Click / TwoFactorDisable_Click)', '', U, runnable=False,
   reason='not in the user interface: commented out in the markup; two-factor sign-in itself is exercised with the flag set in the database (acct.two-factor)')
op('acct.two-factor-no-user', 'account', '/Account/TwoFactorAuthenticationSignIn', 'without a verified user', 'redirect /Account/Error (a 404)', A)
op('acct.two-factor', 'account', '/Account/Login -> TwoFactorAuthenticationSignIn', 'SignInStatus.RequiresVerification; the code (TOTP of EmailTokenProvider)', 'signed in', A, pre='TwoFactorEnabled and EmailConfirmed (set in the database)',
   keys=['GET /account/twofactorauthenticationsignin*'])
op('acct.two-factor-send-code', 'account', '/Account/TwoFactorAuthenticationSignIn', 'DropDownList Providers + Button ProviderSubmit (SendTwoFactorCode)', 'the code form', A,
   keys=['INPUT /account/twofactorauthenticationsignin *', 'SUBMIT /account/twofactorauthenticationsignin *'])
op('acct.two-factor-wrong-code', 'account', '/Account/TwoFactorAuthenticationSignIn', 'Button CodeSubmit with a wrong code', '"Invalid code"', A)
op('acct.two-factor-remember-browser', 'account', '/Account/TwoFactorAuthenticationSignIn', 'CheckBox RememberBrowser', 'the two-factor remember-browser cookie', A)
op('acct.two-factor-lockout', 'account', '/Account/TwoFactorAuthenticationSignIn', 'wrong codes: AccessFailed, MaxFailedAccessAttemptsBeforeLockout 5', 'locked out after the fifth; redirect /Account/Lockout', A)
op('acct.forgot-page', 'account', '/Account/Forgot', 'GET (its link on the login page is commented out)', 'the form', A, keys=['GET /account/forgot'])
op('acct.forgot-required', 'account', '/Account/Forgot', 'RequiredFieldValidator Email', 'the message', A)
op('acct.forgot-unknown', 'account', '/Account/Forgot', 'an unknown user', '"The user either does not exist or is not confirmed."', A)
op('acct.forgot-unconfirmed', 'account', '/Account/Forgot', 'a user whose e-mail is not confirmed', 'the same message', A)
op('acct.forgot', 'account', '/Account/Forgot', 'a confirmed user', '"Please check your email to reset your password." (nothing is sent)', A, pre='EmailConfirmed (set in the database)')
op('acct.confirm-email', 'account', '/Account/Confirm', 'without / with a bogus code', '"An error has occurred."', A, keys=['GET /account/confirm*'])
op('acct.confirm-email-success', 'account', '/Account/Confirm', 'a valid confirmation token', 'the success panel', A, runnable=False,
   reason='the token is made only by commented-out code (Register) and protected with the server\'s data protection: no way to obtain one')
op('acct.reset-password-page', 'account', '/Account/ResetPassword', 'GET', 'the form', A, keys=['GET /account/resetpassword*'])
op('acct.reset-password-required', 'account', '/Account/ResetPassword', 'RequiredFieldValidators', 'the messages', A, keys=['VALIDATOR /account/resetpassword *', 'INPUT /account/resetpassword *', 'SUBMIT /account/resetpassword *'])
op('acct.reset-password-mismatch', 'account', '/Account/ResetPassword', 'CompareValidator', 'the message', A)
op('acct.reset-password-no-code', 'account', '/Account/ResetPassword', 'Button "Reset" without a code', '"An error has occurred"', A)
op('acct.reset-password-unknown-user', 'account', '/Account/ResetPassword?code=', 'an unknown user', '"No user found"', A)
op('acct.reset-password-bad-token', 'account', '/Account/ResetPassword?code=', 'a bogus token', '"Invalid token."', A)
op('acct.reset-password-success', 'account', '/Account/ResetPassword', 'a valid reset token', 'redirect ResetPasswordConfirmation', A, runnable=False,
   reason='the token is made only by commented-out code (Forgot) and protected with the server\'s data protection')
op('acct.reset-password-confirmation', 'account', '/Account/ResetPasswordConfirmation', 'GET', 'the page', A, keys=['GET /account/resetpasswordconfirmation'])

# --- security ---
op('sec.postback-valid', 'security', '/ (any page)', 'a postback of the page\'s own form (view state, anti-XSRF token of Site.Master)', 'the page again')
op('sec.xsrf-cookie', 'security', '/, /Account/Login', 'a postback with another __AntiXsrfToken cookie (Site.Master makes it the ViewStateUserKey)', "500: the view state's MAC fails first")
op('sec.xsrf-default-page-error', 'security', '/', "a postback of a page rendered before the user logged in (in another tab): Site.Master's InvalidOperationException, Default.Page_Error: Server.Transfer to ErrorPage", 'the error page with the handler "Page_Error - Default.aspx"')
op('sec.xsrf-user-changed', 'security', '/Contact', 'the same on a page without Page_Error', '500 "Validation of Anti-XSRF token failed."')
op('sec.viewstate-mac', 'security', '/Account/Login', 'a tampered __VIEWSTATE', '500 (the view state\'s MAC)')
op('sec.event-validation', 'security', '/Account/Login, /Admin/AdminPage', 'a tampered __EVENTVALIDATION; a forged option of a DropDownList', '500 (event validation)')
op('sec.request-validation-form', 'security', '/Account/Login, /Admin/AdminPage', 'markup in a posted value', '500 HttpRequestValidationException')
op('sec.request-validation-query', 'security', '/ErrorPage, /Checkout/CheckoutError', 'markup in the query string', '500 HttpRequestValidationException')

# --- administration ---
op('admin.page', 'admin', '/Admin/AdminPage', 'GET: DropDownLists DropDownAddCategory / DropDownRemoveProduct (SelectMethod, AppendDataBoundItems)', 'the form', ADM,
   keys=['INPUT /admin/adminpage *', 'SUBMIT /admin/adminpage *', 'VALIDATOR /admin/adminpage *'])
op('admin.add-required', 'admin', '/Admin/AdminPage', 'RequiredFieldValidators 1-4 (name, description, price, image: FileUpload) client and server', 'the messages', ADM)
op('admin.add-price-invalid', 'admin', '/Admin/AdminPage', 'RegularExpressionValidator ^[0-9]*(\\.)?[0-9]?[0-9]?$', '"* Must be a valid price without $."', ADM, inputs=['$5'])
op('admin.add-price-unparsable', 'admin', '/Admin/AdminPage', 'a price the expression accepts but Convert.ToDouble does not', '500 FormatException (after the image was saved)', ADM, inputs=['.'])
op('admin.add-bad-file-type', 'admin', '/Admin/AdminPage', 'an image that is not .gif/.png/.jpeg/.jpg', '"Unable to accept file type."', ADM, inputs=['parity-probe.txt'])
op('admin.add-name-too-long', 'admin', '/Admin/AdminPage', 'a name over the model\'s StringLength(100)', '500 DbEntityValidationException', ADM)
op('admin.add-product', 'admin', '/Admin/AdminPage', 'FileUpload ProductImage (SaveAs Catalog/Images and Thumbs) + Button "Add Product" (AddProducts.AddProduct)', 'redirect ?ProductAction=add "Product added!"; the product listed, its image served, its price formatted', ADM,
   inputs=['Boats, Parity Boat, 12.5, parity-boat.png'])
op('admin.status-query', 'admin', '/Admin/AdminPage?ProductAction=', 'ProductAction add / remove', 'the status label', ADM, keys=['GET /admin/adminpage?productaction'])
op('admin.remove-product', 'admin', '/Admin/AdminPage', 'Button "Remove Product" (RemoveProductButton_Click)', 'redirect ?ProductAction=remove "Product removed!"; the product and its cart rows gone (cascade)', ADM)
op('admin.remove-missing', 'admin', '/Admin/AdminPage', 'a product deleted meanwhile', '"Unable to locate product."', ADM, pre='the product deleted in the database after the page was rendered')

# --- state ---
op('state.cookies-anonymous', 'state', 'any page', 'the cookies of an anonymous visit', '__AntiXsrfToken (HttpOnly), the session cookie')
op('state.cookies-authenticated', 'state', 'after login', 'the authentication cookie', '.AspNet.ApplicationCookie (HttpOnly; session or persistent)')

here = os.path.dirname(os.path.abspath(__file__))
ids = [o['id'] for o in OPS]
assert len(ids) == len(set(ids)), 'duplicate ids'
with open(os.path.join(here, 'wt.operations.json'), 'w', encoding='utf-8', newline='\n') as f:
    json.dump({'_comment': 'Generated by wt-operations-inventory.py (edit the table there). The operations of WingtipToys; corpora/regression/wt.operations.scenario.json exercises them (its steps\' op).',
               'operations': OPS}, f, ensure_ascii=False, indent=2)
    f.write('\n')

cats = {}
for o in OPS:
    cats.setdefault(o['category'], []).append(o)
lines = ['# WingtipToys: the inventory of operations', '',
         'Generated by `wt-operations-inventory.py` from the same table as `wt.operations.json` (edit the table there and run it).',
         'Each operation is exercised by the steps of `wt.operations.scenario.json` that name it (`op`); the golden data is recorded on the',
         'original on IIS (`corpora/record-wt-operations.ps1`) and compared on Windows and Linux (`verify-windows.ps1 -Name wt -Suite operations`,',
         '`run-linux.ps1 ... -Suite operations`).', '',
         '| category | operations | not runnable |', '|---|---|---|']
for c, items in cats.items():
    lines.append(f'| {c} | {len(items)} | {sum(1 for i in items if not i["runnable"])} |')
lines.append(f'| **total** | **{len(OPS)}** | **{sum(1 for i in OPS if not i["runnable"])}** |')
for c, items in cats.items():
    lines += ['', f'## {c}', '', '| id | page | element | role / preconditions | inputs | expected |', '|---|---|---|---|---|---|']
    for o in items:
        role = o.get('role', '') + ('; ' + o['preconditions'] if o.get('preconditions') else '')
        exp = o.get('expected', '')
        if not o['runnable']:
            exp = f'**not runnable**: {o["reason"]}'
        elif o.get('partial'):
            exp += f' (partial: {o["partial"]})'
        cells = [o['id'], o['page'], o['element'], role, ', '.join(o.get('inputs', [])), exp]
        lines.append('| ' + ' | '.join(str(x).replace('|', '\\|') for x in cells) + ' |')
with open(os.path.join(here, 'wt.operations.md'), 'w', encoding='utf-8', newline='\n') as f:
    f.write('\n'.join(lines) + '\n')
print(len(OPS), 'operations')
