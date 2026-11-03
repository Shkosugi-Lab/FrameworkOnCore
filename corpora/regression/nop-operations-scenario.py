# Writes corpora/regression/nop.operations.scenario.json, nopCommerce 1.90's operations suite (ParityTest's scenario), from
# the steps below: each operation of the inventory (nop.operations.json, nop-operations-inventory.py) done in the order the
# phases give, with the state checks (the database: sql steps; the cookies) after them. Edit the steps and run:
#   python corpora/regression/nop-operations-scenario.py
# (then record the golden data again on IIS: corpora/record-nop-operations.ps1; tests/README.md). The output is
# deterministic: the same JSON from the same steps. Every run starts from the same database (corpora/nop-database.ps1:
# the installer's sample data, restored from a backup).
import json, os, sys
sys.path.insert(0, os.path.dirname(os.path.abspath(__file__)))
from nop_operations_common import ADMIN_PAGES, GATEWAY_PAGES  # noqa: E402

STEPS = []


def phase(text):
    STEPS.append({'_phase': text})


def s(action, op=None, **kw):
    step = {'action': action}
    step.update({k: v for k, v in kw.items() if v is not None})
    if op:
        step['op'] = op
    STEPS.append(step)


def goto(path, name=None, op=None, error=False):
    s('goto', op, path=path)
    if name:
        s('snapshot', name=name, value='error' if error else None)


def snap(name, error=False, op=None):
    s('snapshot', op, name=name, value='error' if error else None)


def fetch(path, name, op=None, **kw):
    s('fetch', op, path=path, name=name, **kw)


def sql(name, query, op=None):
    s('sql', op, name=name, value=query)


def click(target, op=None):
    s('click', op, target=target)


def fill(target, value, op=None):
    s('fill', op, target=target, value=value)


def fill_tab(target, value, op=None):
    """A field filled after its validator's message was shown: Tab (its change event) first, so the message is gone before
    the next click (the button would move under the click: as a user's click, it would miss)."""
    s('fill', op, target=target, value=value)
    s('press', target=target, value='Tab')


def tab(panel, op=None):
    """A TabContainer's tab (Ajax Control Toolkit) made the active one: its header clicked (a span in 4.1, a link in 20.1)."""
    s('click', op, target=f"css=[id^='__tab_'][id$='_{panel}']")


def script(name, js, op=None):
    s('script', op, name=name, value=js)


# The storefront's master pages: their controls' ids end so (ctl00_ctl00_ctrlHeaderMenu_ctrlSearchBox_txtSearchTerms).
SEARCHBOX = 'ctrlSearchBox_txtSearchTerms'
SEARCHBOX_BUTTON = 'ctrlSearchBox_btnSearch'
CURRENCY = 'ctrlCurrencySelector_ddlCurrencies'


# ---------------------------------------------------------------------------------------------------------------------
phase('1. Anonymous browsing of the sample store (a new browser: no cookies)')
fetch('/', 'root-redirect', 'page.root')
goto('/default.aspx', 'home', 'page.home,page.home-categories,page.home-news,page.home-text,page.header-footer')
s('assets', 'static.page-assets', name='home-assets')
s('cookies', 'state.cookies-anonymous', name='cookies-anonymous')
# Categories: the templates (products in grid 3, in lines 1 5, in lines 2 1) and their sub-categories.
goto('/category/29-books.aspx', 'category-books', 'page.category,page.category-template-grid')
goto('/category/30-computers.aspx', 'category-computers', 'page.category,page.category-subcategories')
goto('/category/50-desktops.aspx', 'category-desktops', 'page.category-template-lines1')
goto('/category/51-notebooks.aspx', 'category-notebooks', 'page.category-template-lines1,page.category-spec-filter')
goto('/category/44-shirts.aspx', 'category-shirts', 'page.category-template-lines1')
goto('/category/49-jewelry.aspx', 'category-jewelry', 'page.category-price-filter')
goto('/category/49-jewelry.aspx?price=500-', 'category-jewelry-price', 'page.category-price-filter')
goto('/category/49-jewelry.aspx?pageindex=2', 'category-jewelry-page2', 'page.category-paging')
goto('/category/51-notebooks.aspx?cputype=31', 'category-notebooks-cpu', 'page.category-spec-filter')
goto('/category/9999-none.aspx', 'category-unknown', 'page.category-unknown')
goto('/category.aspx?categoryid=29', 'category-by-query', 'page.category')
goto('/manufacturers.aspx', 'manufacturers', 'page.manufacturers')
goto('/manufacturer/1-hp.aspx', 'manufacturer-hp', 'page.manufacturer')
goto('/manufacturer/2-asus.aspx?pageindex=1', 'manufacturer-asus', 'page.manufacturer')
# Products: the two templates (one variant; variants in a grid), attributes, tier prices, specifications, reviews,
# related products, gift cards, downloads.
goto('/products/45-black-white-diamond-heart.aspx', 'product-one-variant', 'page.product,page.product-template-onevariant,page.product-tabs')
goto('/products/67-build-your-own-computer.aspx', 'product-attributes', 'page.product-template-variantsingrid,page.product-attributes')
goto('/products/85-canon-digital-rebel-xsi-122-mp-digital-slr-camera.aspx', 'product-two-variants', 'page.product-template-variantsingrid')
goto('/products/64-hp-iq506-touchsmart-desktop-pc.aspx', 'product-specs', 'page.product-specifications,page.product-related')
goto('/products/86-custom-t-shirt.aspx', 'product-text-attribute', 'page.product-attributes')
goto('/products/88-5-virtual-gift-card.aspx', 'product-gift-card', 'page.product-gift-card')
goto('/products/58-single-ladies-put-a-ring-on-it.aspx', 'product-download', 'page.product-download-sample')
goto('/products/9999-none.aspx', 'product-unknown', 'page.product-unknown')
goto('/product.aspx?productid=45', 'product-by-query', 'page.product')
fetch('/getdownload.ashx?sampledownloadproductvariantid=67', 'download-sample', 'handler.getdownload-sample', hash=True, show=['Content-Disposition'])
fetch('/getdownload.ashx?sampledownloadproductvariantid=54', 'download-sample-none', 'handler.getdownload-sample')
goto('/producttag/2-awesome.aspx', 'product-tag', 'page.product-tag')
goto('/producttag/2-awesome.aspx?pageindex=2', 'product-tag-page2', 'page.product-tag')
goto('/recentlyaddedproducts.aspx', 'recently-added', 'page.recently-added')
fetch('/recentlyaddedproductsrss.aspx', 'recently-added-rss', 'page.recently-added-rss', content='xml')
goto('/recentlyviewedproducts.aspx', 'recently-viewed', 'page.recently-viewed')
goto('/sitemap.aspx', 'sitemap', 'page.sitemap')
fetch('/sitemapseo.aspx', 'sitemap-seo', 'page.sitemap-seo', content='xml')
for slug, name in [('6-about-us', 'about-us'), ('3-conditions-of-use', 'conditions'), ('4-privacy-policy', 'privacy'), ('5-shipping-returns', 'shipping-returns')]:
    goto(f'/topic/{slug}.aspx', f'topic-{name}', 'page.topic')
goto('/aboutus.aspx', 'aboutus', 'page.topic-pages')
goto('/conditionsinfo.aspx', 'conditionsinfo', 'page.topic-pages')
goto('/conditionsinfopopup.aspx', 'conditionsinfopopup', 'page.topic-pages')
goto('/privacyinfo.aspx', 'privacyinfo', 'page.topic-pages')
goto('/shippinginfo.aspx', 'shippinginfo', 'page.topic-pages')
goto('/useragreement.aspx', 'useragreement', 'page.user-agreement')
goto('/topic/99-none.aspx', 'topic-unknown', 'page.topic')
goto('/blog.aspx', 'blog', 'blog.list')
goto('/blog.aspx?tag=money', 'blog-tag', 'blog.tag')
goto('/blog.aspx?month=2010-4', 'blog-month', 'blog.month')
goto('/blog/5-online-discount-coupons.aspx', 'blog-post', 'blog.post')
fetch('/blogrss.aspx?languageid=7', 'blog-rss', 'blog.rss', content='xml')
goto('/newsarchive.aspx', 'news-archive', 'news.archive')
goto('/news/9-new-online-store-is-open.aspx', 'news-item', 'news.item')
fetch('/newsrss.aspx?languageid=7', 'news-rss', 'news.rss', content='xml')
goto('/compareproducts.aspx', 'compare-empty', 'cart.compare')
goto('/shoppingcart.aspx', 'cart-empty', 'cart.view')
goto('/wishlist.aspx', 'wishlist-empty', 'wishlist.view')
goto('/contactus.aspx', 'contact', 'page.contact')
goto('/login.aspx', 'login-page', 'acct.login-page')
goto('/register.aspx', 'register-page', 'acct.register-page')
goto('/passwordrecovery.aspx', 'password-recovery-page', 'acct.password-recovery-page')
goto('/account.aspx', 'account-anonymous', 'acct.account-anonymous')
goto('/search.aspx', 'search-page', 'search.page')
fetch('/froogle.ashx', 'froogle', 'handler.froogle', content='xml')
fetch('/robots.txt', 'robots', 'static.files', content='text')
fetch('/keepalive/ping.ashx', 'keepalive', 'handler.keepalive', content='text')
fetch('/captchaimage.aspx', 'captcha-image', 'handler.captcha-image')
fetch('/errorpage.htm', 'static-errorpage', 'static.files', hash=True)
fetch('/filenotfound.htm', 'static-filenotfound', 'static.files', hash=True)
fetch('/no-such-page.aspx', 'missing-aspx', 'error.not-found')
fetch('/no-such-file.txt', 'missing-file', 'error.not-found')

# Folders and extensionless URLs: the managed pipeline runs for them as on IIS (UrlRewritingNet's redirect to default.aspx).
for path, name in [('/administration', 'folder-admin'), ('/administration/', 'folder-admin-slash'), ('/Boards', 'folder-boards'),
                   ('/images/', 'folder-images'), ('/foo', 'extensionless-none'), ('/?a=1', 'root-query')]:
    fetch(path, name, 'error.folders')

# ---------------------------------------------------------------------------------------------------------------------
phase('2. Anonymous shopping: search, currency, compare, cart, wishlist, newsletter, contact')
# The header's search box; the search page's simple and advanced search (category, manufacturer, prices, descriptions), its pager.
goto('/default.aspx')
fill(SEARCHBOX, 'computer')
click(SEARCHBOX_BUTTON, 'search.header')
snap('search-header')
goto('/search.aspx?searchterms=laptop', 'search-query', 'search.query')
goto('/search.aspx')
fill('ctrlSearch_txtSearchTerm', 'the')
click('ctrlSearch_btnSearch', 'search.simple')
snap('search-simple')
s('check', target='ctrlSearch_cbAdvancedSearch')
s('check', target='ctrlSearch_cbSearchInProductDescriptions')
click('ctrlSearch_btnSearch', 'search.descriptions')
snap('search-descriptions')
# The DataPager's buttons (NextPreviousPagerField): next, last, previous, first.
s('click', 'search.paging', target="css=input[type=submit][value='Next']")
snap('search-page2')
s('click', 'search.paging', target="css=input[type=submit][value='Last']")
snap('search-last')
s('click', 'search.paging', target="css=input[type=submit][value='Previous']")
s('click', 'search.paging', target="css=input[type=submit][value='First']")
snap('search-first')
goto('/search.aspx')
fill('ctrlSearch_txtSearchTerm', 'x')
click('ctrlSearch_btnSearch', 'search.too-short')
snap('search-too-short')
goto('/search.aspx')
fill('ctrlSearch_txtSearchTerm', 'nothing-matches-this')
click('ctrlSearch_btnSearch', 'search.no-results')
snap('search-no-results')
goto('/search.aspx')
fill('ctrlSearch_txtSearchTerm', 'pc')
s('check', target='ctrlSearch_cbAdvancedSearch')
s('select', target='ctrlSearch_ddlCategories', value='30')
s('select', target='ctrlSearch_ddlManufacturers', value='1')
fill('ctrlSearch_txtPriceFrom', '100')
fill('ctrlSearch_txtPriceTo', '1500')
s('check', target='ctrlSearch_cbSearchInProductDescriptions')
click('ctrlSearch_btnSearch', 'search.advanced')
snap('search-advanced')
sql('search-log', 'SELECT SearchTerm FROM Nop_SearchLog ORDER BY SearchLogID', 'search.log')
# The currency selector (AutoPostBack): prices in euros; back to dollars.
goto('/category/29-books.aspx')
s('select', 'page.currency', target=CURRENCY, value='7')
snap('currency-euro')
goto('/products/45-black-white-diamond-heart.aspx', 'product-euro', 'page.currency')
s('select', 'page.currency', target=CURRENCY, value='1')
snap('currency-dollar')
# Compare products: added from the product pages, the list, cleared.
goto('/products/64-hp-iq506-touchsmart-desktop-pc.aspx')
click('ctrlProductAddToCompareList_btnAddToCompareList', 'cart.compare-add')
snap('compare-one')
goto('/products/65-hp-pavilion-elite-m9150f-desktop-pc.aspx')
click('ctrlProductAddToCompareList_btnAddToCompareList', 'cart.compare-add')
snap('compare-two')
s('cookies', 'cart.compare-add', name='cookies-compare')
click('btnClearCompareProductsList', 'cart.compare-clear')
snap('compare-cleared')
# Product actions an anonymous visitor may not do (the settings): e-mail a friend, write a review.
goto('/products/45-black-white-diamond-heart.aspx')
click('ctrlProductEmailAFriendButton_btnEmailAFriend', 'product.email-friend-anonymous')
snap('email-friend-anonymous')
goto('/products/45-black-white-diamond-heart.aspx')
click('ctrlProductReviews_btnWriteReview', 'product.review-anonymous')
snap('review-anonymous')
goto('/productreviewadd.aspx?productid=45', 'review-add-anonymous', 'product.review-anonymous')
goto('/productemailafriend.aspx?productid=45', 'email-friend-page-anonymous', 'product.email-friend-anonymous')
# Add to cart: one variant (quantity validation), attributes required, a text attribute, a variant of the grid.
goto('/products/45-black-white-diamond-heart.aspx')
fill('txtQuantity_txtValue', '')
click('btnAddToCart', 'cart.add-quantity-validation')
snap('add-quantity-required')
fill_tab('txtQuantity_txtValue', '2')
click('btnAddToCart', 'cart.add-one-variant')
snap('cart-after-add')
sql('cart-rows-1', 'SELECT ShoppingCartTypeID, ProductVariantID, AttributesXML, CustomerEnteredPrice, Quantity FROM Nop_ShoppingCartItem ORDER BY ShoppingCartItemID', 'cart.add-one-variant')
goto('/products/67-build-your-own-computer.aspx')
click('ctrlProductVariantsInGrid_rptVariants_ctl00_btnAddToCart', 'cart.add-attributes-required')
snap('add-attributes-required')
script(None, "document.querySelectorAll(\"select[id*='ctrlProductAttributes']\").forEach(s => { s.selectedIndex = s.options.length - 1; }); document.querySelectorAll(\"input[type=radio][id*='ctrlProductAttributes']\").forEach((r, i, all) => { if (r.id.endsWith('_0')) r.checked = true; }); document.querySelectorAll(\"input[type=checkbox][id*='ctrlProductAttributes']\").forEach(c => { c.checked = true; });")
click('ctrlProductVariantsInGrid_rptVariants_ctl00_btnAddToCart', 'cart.add-attributes')
snap('cart-after-attributes')
goto('/products/86-custom-t-shirt.aspx')
s('script', value="document.querySelectorAll(\"input[type=text][id*='ctrlProductAttributes']\").forEach(t => { t.value = 'Parity text'; });")
click('btnAddToCart', 'cart.add-text-attribute')
snap('cart-after-text-attribute')
goto('/products/85-canon-digital-rebel-xsi-122-mp-digital-slr-camera.aspx')
click('ctrlProductVariantsInGrid_rptVariants_ctl01_btnAddToCart', 'cart.add-variant-grid')
snap('cart-after-variant-grid')
goto('/products/88-5-virtual-gift-card.aspx')
click('btnAddToCart', 'cart.add-gift-card-validation')
snap('add-gift-card-required')
fill('txtRecipientName', 'Parity Friend')
fill('txtRecipientEmail', 'friend@example.com')
fill('txtSenderName', 'Parity Visitor')
fill('txtSenderEmail', 'visitor@example.com')
fill_tab('txtGiftCardMessage', 'For you')
click('btnAddToCart', 'cart.add-gift-card')
snap('cart-after-gift-card')
goto('/category/29-books.aspx')
s('clicktextin', 'cart.add-from-list', target='css=.product-grid, .product-list', value='Add to cart')
snap('cart-after-list-add')
sql('cart-rows-2', 'SELECT ShoppingCartTypeID, ProductVariantID, AttributesXML, CustomerEnteredPrice, Quantity FROM Nop_ShoppingCartItem ORDER BY ShoppingCartItemID', 'cart.add-attributes,cart.add-text-attribute,cart.add-variant-grid,cart.add-gift-card,cart.add-from-list')
goto('/shoppingcart.aspx', 'cart', 'cart.view')
# The cart: quantities updated, an item removed, the discount coupon (sample: 123, 456; a wrong one), a gift card code (none), the
# shipping estimate (country -> states, a zip code), continue shopping.
s('script', value="const q = document.querySelectorAll(\"input[id$='_txtQuantity']\"); if (q.length) q[0].value = '3';")
click('btnUpdate', 'cart.update-quantity')
snap('cart-quantity-updated')
s('script', value="const c = document.querySelectorAll(\"input[type=checkbox][id$='_cbRemoveFromCart']\"); if (c.length > 1) c[c.length - 1].checked = true;")
click('btnUpdate', 'cart.remove')
snap('cart-item-removed')
fill('txtDiscountCouponCode', 'wrong')
click('btnApplyDiscountCouponCode', 'cart.discount-wrong')
snap('cart-discount-wrong')
fill('txtDiscountCouponCode', '456')
click('btnApplyDiscountCouponCode', 'cart.discount-apply')
snap('cart-discount-456')
fill('txtDiscountCouponCode', '123')
click('btnApplyDiscountCouponCode', 'cart.discount-apply')
snap('cart-discount-123')
fill('txtGiftCardCouponCode', 'no-such-card')
click('btnApplyGiftCardsCouponCode', 'cart.gift-card-wrong')
snap('cart-gift-card-wrong')
s('select', 'cart.estimate-shipping', target='ctrlEstimateShipping_ddlCountry', value='1')
fill('ctrlEstimateShipping_txtZipPostalCode_txtValue', '10021')
click('btnGetQuote', 'cart.estimate-shipping')
snap('cart-estimate-shipping')
click('btnContinueShopping', 'cart.continue-shopping')
snap('cart-continue-shopping')
goto('/shoppingcart.aspx')
click('btnCheckout', 'checkout.anonymous-to-login')
snap('checkout-anonymous')
# Wishlist: added from a product, quantities, moved to the cart, removed; e-mailed (anonymous: not allowed).
goto('/products/46-diamond-tennis-bracelet.aspx')
click('btnAddToWishlist', 'wishlist.add')
snap('wishlist-after-add')
goto('/products/56-ray-ban-aviator-sunglasses-rb-3025.aspx')
click('btnAddToWishlist', 'wishlist.add')
s('script', value="const q = document.querySelectorAll(\"input[id$='_txtQuantity']\"); if (q.length) q[0].value = '2';")
click('btnUpdate', 'wishlist.update')
snap('wishlist-updated')
s('script', value="const c = document.querySelectorAll(\"input[type=checkbox][id$='_cbAddToCart']\"); if (c.length) c[0].checked = true;")
click('btnAddToCart', 'wishlist.to-cart')
snap('wishlist-to-cart')
goto('/wishlist.aspx')
s('script', value="document.querySelectorAll(\"input[type=checkbox][id$='_cbRemoveFromCart']\").forEach(c => c.checked = true);")
click('btnUpdate', 'wishlist.remove')
snap('wishlist-removed')
goto('/wishlistemailafriend.aspx', 'wishlist-email-anonymous', 'wishlist.email-anonymous')
sql('cart-rows-3', 'SELECT ShoppingCartTypeID, ProductVariantID, AttributesXML, CustomerEnteredPrice, Quantity FROM Nop_ShoppingCartItem ORDER BY ShoppingCartItemID', 'cart.update-quantity,cart.remove,wishlist.add,wishlist.update,wishlist.to-cart,wishlist.remove')
# The newsletter box: validation, subscribe (an inactive subscription and its activation e-mail), the activation link, unsubscribe.
goto('/default.aspx')
click('ctrlNewsLetterSubscriptionBoxControl_btnSubscribe', 'newsletter.validation')
snap('newsletter-required')
fill_tab('ctrlNewsLetterSubscriptionBoxControl_txtEmail_txtValue', 'news@example.com')
click('ctrlNewsLetterSubscriptionBoxControl_btnSubscribe', 'newsletter.subscribe')
snap('newsletter-subscribed')
sql('newsletter-rows', 'SELECT Email, Active FROM Nop_NewsLetterSubscription ORDER BY NewsLetterSubscriptionID', 'newsletter.subscribe')
sql('queued-email-newsletter', 'SELECT [From], FromName, [To], ToName, Subject, Body FROM Nop_QueuedEmail ORDER BY QueuedEmailID', 'newsletter.subscribe')
# The activation link of the e-mail (its GUID made a known one first: the link is in the e-mail only), an unknown one.
s('sql', value="UPDATE Nop_NewsLetterSubscription SET NewsLetterSubscriptionGuid = '00000000-0000-0000-0000-0000000000a1' WHERE Email = 'news@example.com'")
goto('/newslettersubscriptionactivation.aspx?t=00000000-0000-0000-0000-0000000000a1&active=1', 'newsletter-activated', 'newsletter.activate')
goto('/newslettersubscriptionactivation.aspx?t=00000000-0000-0000-0000-0000000000a1&active=0', 'newsletter-deactivated', 'newsletter.activate')
goto('/newslettersubscriptionactivation.aspx?t=00000000-0000-0000-0000-0000000000ff&active=1', 'newsletter-activation-unknown', 'newsletter.activate')
sql('newsletter-rows-activation', 'SELECT Email, Active FROM Nop_NewsLetterSubscription ORDER BY NewsLetterSubscriptionID', 'newsletter.activate')
goto('/default.aspx')
fill('ctrlNewsLetterSubscriptionBoxControl_txtEmail_txtValue', 'news@example.com')
s('check', target='ctrlNewsLetterSubscriptionBoxControl_rbUnsubscribe')
click('ctrlNewsLetterSubscriptionBoxControl_btnSubscribe', 'newsletter.unsubscribe')
snap('newsletter-unsubscribed')
sql('newsletter-rows-2', 'SELECT Email, Active FROM Nop_NewsLetterSubscription ORDER BY NewsLetterSubscriptionID', 'newsletter.unsubscribe')
# The contact form: validation, sent (the e-mail queued).
goto('/contactus.aspx')
click('btnContactUs', 'page.contact-validation')
snap('contact-required')
fill('txtFullName_txtValue', 'Parity Visitor')
fill('ctrlContactUs_txtEmail_txtValue', 'visitor@example.com')
fill_tab('txtEnquiry', 'A question from the parity test.')
click('btnContactUs', 'page.contact-send')
snap('contact-sent')
sql('queued-email-contact', 'SELECT [From], FromName, [To], ToName, Subject, Body FROM Nop_QueuedEmail ORDER BY QueuedEmailID', 'page.contact-send')

CUSTOMERS = ("SELECT c.Email, c.Username, c.IsAdmin, c.IsGuest, c.Active, c.Deleted, c.LanguageID, c.CurrencyID, c.TaxDisplayTypeID, c.AvatarID, "
             "(SELECT COUNT(*) FROM Nop_Address a WHERE a.CustomerID = c.CustomerID) AS Addresses, "
             "(SELECT COUNT(*) FROM Nop_ShoppingCartItem i JOIN Nop_CustomerSession s ON s.CustomerSessionGUID = i.CustomerSessionGUID WHERE s.CustomerID = c.CustomerID) AS CartItems "
             "FROM Nop_Customer c WHERE c.IsGuest = 0 ORDER BY c.CustomerID")
ATTRIBUTES = ("SELECT c.Email, a.[Key], a.Value FROM Nop_CustomerAttribute a JOIN Nop_Customer c ON c.CustomerID = a.CustomerId "
              "WHERE c.IsGuest = 0 AND a.[Key] NOT IN ('PasswordRecoveryToken', 'AccountActivationToken') ORDER BY c.CustomerID, a.[Key]")
ADDRESSES = ("SELECT c.Email, a.IsBillingAddress, a.FirstName, a.LastName, a.PhoneNumber, a.Email AS AddressEmail, a.FaxNumber, a.Company, a.Address1, a.Address2, "
             "a.City, a.StateProvinceID, a.ZipPostalCode, a.CountryID FROM Nop_Address a JOIN Nop_Customer c ON c.CustomerID = a.CustomerID "
             "WHERE c.IsGuest = 0 ORDER BY a.AddressId")
# The e-mails' bodies without their CRs: nopCommerce writes the lines of a body's tables with StringBuilder.AppendLine
# (Environment.NewLine: CR LF on Windows, LF on Linux; the templates' own lines are CR LF everywhere). HTML: the same mail.
QUEUE = 'SELECT [From], FromName, [To], ToName, Subject, REPLACE(Body, CHAR(13), \'\') AS Body FROM Nop_QueuedEmail ORDER BY QueuedEmailID'
REG = 'CreateUserStepContainer_'

# ---------------------------------------------------------------------------------------------------------------------
phase('3. Registration, login, password recovery, the account pages (the anonymous cart carried over)')
goto('/register.aspx')
click('StepNextButton', 'acct.register-validation')
snap('register-required')
fill(REG + 'txtFirstName', 'Parity')
fill(REG + 'txtLastName', 'Customer')
s('check', target=REG + 'rbGenderF')
fill(REG + 'UserName', 'not-an-email')
fill(REG + 'txtCompany', 'Parity Ltd')
fill(REG + 'txtStreetAddress', '1 Test Street')
fill(REG + 'txtZipPostalCode', '10021')
fill(REG + 'txtCity', 'New York')
s('select', target=REG + 'ddlCountry', value='1')
fill(REG + 'txtPhoneNumber', '12345678')
fill(REG + 'Password', 'Parity-1')
fill_tab(REG + 'ConfirmPassword', 'Parity-2')
click('StepNextButton', 'acct.register-validation')
snap('register-invalid-email-mismatch')
fill(REG + 'UserName', 'parity@example.com')
fill(REG + 'Password', 'Parity-1')
fill_tab(REG + 'ConfirmPassword', 'Parity-1')
s('check', target=REG + 'cbNewsletter')
click('StepNextButton', 'acct.register')
snap('registered')
s('cookies', 'acct.register,state.cookies-authenticated', name='cookies-registered')
sql('customers-after-register', CUSTOMERS, 'acct.register')
sql('attributes-after-register', ATTRIBUTES, 'acct.register')
sql('addresses-after-register', ADDRESSES, 'acct.register')
click('ContinueButton', 'acct.register-continue')
snap('register-continue')
goto('/shoppingcart.aspx', 'cart-after-register', 'cart.carried-over')
s('clicktext', 'acct.logout', value='Log out')
snap('logged-out')
s('cookies', 'acct.logout', name='cookies-logged-out')
# A second browser: the same e-mail registered again; a wrong password; the right one (remember me).
s('newcontext')
goto('/register.aspx')
fill(REG + 'txtFirstName', 'Other')
fill(REG + 'txtLastName', 'Customer')
fill(REG + 'UserName', 'parity@example.com')
fill(REG + 'txtStreetAddress', '2 Test Street')
fill(REG + 'txtZipPostalCode', '10021')
fill(REG + 'txtCity', 'New York')
fill(REG + 'txtPhoneNumber', '12345678')
fill(REG + 'Password', 'Parity-1')
fill(REG + 'ConfirmPassword', 'Parity-1')
click('StepNextButton', 'acct.register-duplicate')
snap('register-duplicate')
goto('/login.aspx')
click('LoginButton', 'acct.login-validation')
snap('login-required')
fill('LoginForm_UserName', 'parity@example.com')
fill_tab('LoginForm_Password', 'wrong')
click('LoginButton', 'acct.login-wrong')
snap('login-wrong')
fill('LoginForm_UserName', 'nobody@example.com')
fill('LoginForm_Password', 'Parity-1')
click('LoginButton', 'acct.login-unknown')
snap('login-unknown')
fill('LoginForm_UserName', 'parity@example.com')
fill('LoginForm_Password', 'Parity-1')
s('check', target='LoginForm_RememberMe')
click('LoginButton', 'acct.login')
snap('logged-in')
s('cookies', 'acct.login,state.cookies-authenticated', name='cookies-logged-in-remember')
goto('/login.aspx?returnurl=%2faccount.aspx', 'login-page-authenticated', 'acct.login-page')
# Password recovery: an unknown e-mail, the customer's (its e-mail queued), the link with the token (made a known one: it is
# in the e-mail only), a wrong token, the new password (mismatch, then set), login with it.
s('newcontext')
goto('/passwordrecovery.aspx')
fill('txtEmail_txtValue', 'nobody@example.com')
click('btnPasswordRecovery', 'acct.password-recovery-unknown')
snap('password-recovery-unknown')
goto('/passwordrecovery.aspx')
fill('txtEmail_txtValue', 'parity@example.com')
click('btnPasswordRecovery', 'acct.password-recovery-send')
snap('password-recovery-sent')
sql('queued-email-recovery', QUEUE, 'acct.password-recovery-send')
s('sql', value="UPDATE a SET Value = '00000000-0000-0000-0000-0000000000b1' FROM Nop_CustomerAttribute a JOIN Nop_Customer c ON c.CustomerID = a.CustomerId WHERE c.Email = 'parity@example.com' AND a.[Key] = 'PasswordRecoveryToken'")
goto('/passwordrecovery.aspx?prt=00000000-0000-0000-0000-0000000000ff&email=parity@example.com', 'password-recovery-wrong-token', 'acct.password-recovery-token')
goto('/passwordrecovery.aspx?prt=00000000-0000-0000-0000-0000000000b1&email=parity@example.com', 'password-recovery-token', 'acct.password-recovery-token')
fill('txtNewPassword', 'Parity-3')
fill_tab('txtConfirmNewPassword', 'Parity-4')
click('btnNewPassword', 'acct.password-recovery-new')
snap('password-recovery-mismatch')
fill('txtNewPassword', 'Parity-3')
fill_tab('txtConfirmNewPassword', 'Parity-3')
click('btnNewPassword', 'acct.password-recovery-new')
snap('password-recovery-done')
goto('/login.aspx')
fill('LoginForm_UserName', 'parity@example.com')
fill('LoginForm_Password', 'Parity-3')
click('LoginButton', 'acct.password-recovery-new')
snap('login-new-password')
# The account page: its tabs; the customer's info saved; addresses added, edited, deleted; the password changed.
goto('/account.aspx', 'account', 'acct.account')
fill('ctrlCustomerInfo_txtFirstName', 'Parity2')
fill('ctrlCustomerInfo_txtCity', 'Boston')
click('btnSaveCustomerInfo', 'acct.info-save')
snap('account-info-saved')
sql('attributes-after-info', ATTRIBUTES, 'acct.info-save')
tab('pnlCustomerAddresses', 'acct.account-tabs')
click('btnAddBillingAddress', 'acct.address-add')
snap('address-add-page')
click('btnSave', 'acct.address-validation')
snap('address-required')
A = 'AddressEditControl_'
fill(A + 'txtFirstName_txtValue', 'Bill')
fill(A + 'txtLastName_txtValue', 'Payer')
fill(A + 'txtPhoneNumber_txtValue', '5550100')
fill(A + 'txtEmail_txtValue', 'bill@example.com')
fill(A + 'txtCompany', 'Parity Ltd')
fill(A + 'txtAddress1_txtValue', '10 Billing Road')
fill(A + 'txtCity_txtValue', 'Chicago')
s('select', target=A + 'ddlCountry', value='1')
fill_tab(A + 'txtZipPostalCode_txtValue', '60601')
click('btnSave', 'acct.address-add')
snap('address-added')
tab('pnlCustomerAddresses')
click('btnAddShippingAddress', 'acct.address-add')
fill(A + 'txtFirstName_txtValue', 'Ship')
fill(A + 'txtLastName_txtValue', 'Receiver')
fill(A + 'txtPhoneNumber_txtValue', '5550101')
fill(A + 'txtEmail_txtValue', 'ship@example.com')
fill(A + 'txtAddress1_txtValue', '20 Shipping Lane')
fill(A + 'txtCity_txtValue', 'Seattle')
s('select', target=A + 'ddlCountry', value='2')
fill_tab(A + 'txtZipPostalCode_txtValue', 'K1A 0B1')
click('btnSave', 'acct.address-add')
snap('address-shipping-added')
sql('addresses-after-add', ADDRESSES, 'acct.address-add')
tab('pnlCustomerAddresses')
s('clicktextin', 'acct.address-edit', target='css=.address-list, .customer-addresses, table', value='Edit')
snap('address-edit-page')
fill(A + 'txtCity_txtValue', 'Evanston')
click('btnSave', 'acct.address-edit')
snap('address-edited')
tab('pnlCustomerAddresses')
s('clicktextin', 'acct.address-delete', target='css=.address-list, .customer-addresses, table', value='Delete')
snap('address-deleted')
sql('addresses-after-edit-delete', ADDRESSES, 'acct.address-edit,acct.address-delete')
goto('/account.aspx')
tab('pnlChangePassword', 'acct.account-tabs')
fill('txtOldPassword', 'wrong')
fill('txtNewPassword', 'Parity-5')
fill_tab('txtConfirmPassword', 'Parity-5')
click('btnChangePassword', 'acct.change-password-wrong')
snap('change-password-wrong')
tab('pnlChangePassword')
fill('txtOldPassword', 'Parity-3')
fill('txtNewPassword', 'Parity-5')
fill_tab('txtConfirmPassword', 'Parity-6')
click('btnChangePassword', 'acct.change-password-validation')
snap('change-password-mismatch')
tab('pnlChangePassword')
fill('txtOldPassword', 'Parity-3')
fill('txtNewPassword', 'Parity-1')
fill_tab('txtConfirmPassword', 'Parity-1')
click('btnChangePassword', 'acct.change-password')
snap('change-password-done')
goto('/addressedit.aspx?addressid=77', 'address-edit-other-customer', 'acct.address-other')
goto('/orderdetails.aspx?orderid=1', 'order-details-none', 'acct.order-other')

ORDERS = ("SELECT o.OrderID, c.Email, o.OrderStatusID, o.ShippingStatusID, o.PaymentStatusID, o.PaymentMethodName, o.OrderSubtotalInclTax, o.OrderShippingInclTax, "
          "o.OrderTax, o.OrderTotal, o.OrderDiscount, o.ShippingMethod, o.CardType, o.CardName, o.MaskedCreditCardNumber, o.PurchaseOrderNumber, o.ShippingCountry, "
          "o.BillingEmail, o.Deleted, (SELECT COUNT(*) FROM Nop_OrderProductVariant v WHERE v.OrderID = o.OrderID) AS Lines "
          "FROM Nop_Order o JOIN Nop_Customer c ON c.CustomerID = o.CustomerID ORDER BY o.OrderID")
ORDER_LINES = ("SELECT v.OrderID, v.ProductVariantID, v.Quantity, v.UnitPriceInclTax, v.PriceInclTax, v.AttributeDescription, v.DownloadCount, v.IsDownloadActivated "
               "FROM Nop_OrderProductVariant v ORDER BY v.OrderProductVariantID")
ORDER_NOTES = 'SELECT OrderID, Note, DisplayToCustomer FROM Nop_OrderNote ORDER BY OrderNoteID'


def checkout_steps(payment, fill_payment=None, name='checkout', op_payment='checkout.payment-method'):
    """The multi-step checkout from the cart: shipping address (the customer's first), billing address (the same), shipping
    method (the first), payment method (value), payment info, confirm; a snapshot of each step."""
    goto('/shoppingcart.aspx')
    click('btnCheckout', 'checkout.start')
    snap(f'{name}-shipping-address')
    click('dlShippingAddresses_ctl00_btnSelect', 'checkout.shipping-address-select')
    snap(f'{name}-billing-address')
    click('dlBillingAddresses_ctl00_btnSelect', 'checkout.billing-address-select')
    snap(f'{name}-shipping-method')
    # The sample's methods: In-Store Pickup, By Ground, By Air (Shipping By Order Weight).
    s('check', 'checkout.shipping-method', target='dlShippingOptions_ctl01_rdShippingOption')
    click('btnNextStep', 'checkout.shipping-method')
    snap(f'{name}-payment-method')
    # The active methods in their order: Credit Card (manual), Purchase Order, Cash On Delivery, Check / Money Order, Pay In Store.
    s('check', op_payment, target=f'dlPaymentMethod_ctl0{payment}_rdPaymentMethod')
    click('btnNextStep', op_payment)
    snap(f'{name}-payment-info')
    if fill_payment:
        fill_payment()
    click('btnNextStep', 'checkout.payment-info')
    snap(f'{name}-confirm')
    click('btnNextStep', 'checkout.confirm')
    snap(f'{name}-completed')


def manual_card():
    s('select', target='ddlCreditCardType', value='Visa')
    fill('creditCardName', 'Parity Customer')
    fill('creditCardNumber', '4111111111111111')
    s('select', target='creditCardExpireMonth', value='12')
    s('script', value="const y = document.querySelector(\"select[id$='creditCardExpireYear']\"); y.selectedIndex = y.options.length - 1;")
    fill('creditCardCVV2', '123')


# ---------------------------------------------------------------------------------------------------------------------
phase('4. Checkout (multi-step) as the registered customer: every step, a validation, the manual card payment')
goto('/checkoutshippingaddress.aspx', 'checkout-direct-shipping-address', 'checkout.shipping-address')
# A new shipping address entered in the checkout (its validation first).
click('btnNextStep', 'checkout.shipping-address-validation')
snap('checkout-shipping-address-required')
checkout_steps('0', manual_card, 'checkout-manual', 'checkout.payment-manual')
sql('orders-1', ORDERS, 'checkout.confirm')
sql('order-lines-1', ORDER_LINES, 'checkout.confirm')
sql('order-notes-1', ORDER_NOTES, 'checkout.confirm')
sql('queued-email-order-1', QUEUE, 'checkout.confirm')
sql('cart-after-order-1', 'SELECT ShoppingCartTypeID, ProductVariantID, Quantity FROM Nop_ShoppingCartItem ORDER BY ShoppingCartItemID', 'checkout.confirm')
s('clicktext', 'checkout.completed-details', value='Click here for order details.')
snap('checkout-completed-order-details')

REVIEWS = 'SELECT p.ProductID, c.Email, r.Title, r.ReviewText, r.Rating, r.HelpfulYesTotal, r.HelpfulNoTotal, r.IsApproved FROM Nop_ProductReview r JOIN Nop_Product p ON p.ProductID = r.ProductID JOIN Nop_Customer c ON c.CustomerID = r.CustomerID ORDER BY r.ProductReviewID'
RATINGS = 'SELECT p.ProductID, p.RatingSum, p.TotalRatingVotes FROM Nop_Product p WHERE p.TotalRatingVotes > 0 ORDER BY p.ProductID'

# ---------------------------------------------------------------------------------------------------------------------
phase('5. The customer after the order: orders, invoice, re-order, returns; reviews, ratings, e-mail a friend, comments, polls')
goto('/account.aspx')
tab('pnlCustomerOrders', 'acct.orders')
snap('account-orders')
tab('pnlRewardPoints', 'acct.reward-points')
snap('account-reward-points')
tab('pnlDP', 'acct.downloadable-products')
snap('account-downloadable-products')
goto('/orderdetails.aspx?orderid=1', 'order-details', 'acct.order-details')
s('download', 'acct.order-pdf', target='lbPDFInvoice', name='order-pdf-invoice', content='pdf')
goto('/printorderdetails.aspx?orderid=1', 'order-print', 'acct.order-print')
goto('/returnitems.aspx?orderid=1', 'return-items-not-complete', 'acct.return-items-page')
goto('/orderdetails.aspx?orderid=1')
click('btnReOrder', 'acct.reorder')
snap('reorder-cart')
sql('cart-after-reorder', 'SELECT ShoppingCartTypeID, ProductVariantID, AttributesXML, Quantity FROM Nop_ShoppingCartItem ORDER BY ShoppingCartItemID', 'acct.reorder')
# Product reviews (pending approval), their helpfulness, the rating stars (AutoPostBack), e-mail a friend.
goto('/productreviewadd.aspx?productid=45')
click('btnReview', 'product.review-validation')
snap('review-required')
fill('txtProductReviewTitle_txtValue', 'Parity review')
fill_tab('txtProductReviewText', 'A review written by the parity test.')
s('check', target='rblRating_3')
click('btnReview', 'product.review-add')
snap('review-added')
sql('reviews', REVIEWS, 'product.review-add')
goto('/products/45-black-white-diamond-heart.aspx')
click('ctrlProductRating_productRating_Star_4', 'product.rating')
s('waitms', ms=1500)
snap('product-rated')
sql('ratings', RATINGS, 'product.rating')
goto('/productemailafriend.aspx?productid=45')
click('btnEmail', 'product.email-friend-validation')
snap('email-friend-required')
fill('txtFriendsEmail_txtValue', 'friend@example.com')
fill_tab('txtPersonalMessage', 'Look at this.')
click('btnEmail', 'product.email-friend')
snap('email-friend-sent')
# The wishlist e-mailed to a friend, the shared wishlist (its customer GUID made a known one: it is in the e-mail).
goto('/products/46-diamond-tennis-bracelet.aspx')
click('btnAddToWishlist', 'wishlist.add')
goto('/wishlist.aspx')
click('btnEmailWishlist', 'wishlist.email')
snap('wishlist-email-page')
fill('txtFriendsEmail_txtValue', 'friend@example.com')
fill('txtPersonalMessage', 'My wishlist.')
click('btnEmail', 'wishlist.email')
snap('wishlist-email-sent')
s('sql', value="UPDATE Nop_Customer SET CustomerGUID = '00000000-0000-0000-0000-0000000000c1' WHERE Email = 'parity@example.com'")
s('newcontext')
goto('/wishlist.aspx?customerguid=00000000-0000-0000-0000-0000000000c1', 'wishlist-shared', 'wishlist.shared')
goto('/wishlist.aspx?customerguid=00000000-0000-0000-0000-0000000000ff', 'wishlist-shared-unknown', 'wishlist.shared')
# Comments (anonymous allowed by the sample's settings): a blog post's, a news item's; the poll in the side column.
goto('/blog/5-online-discount-coupons.aspx')
click('btnComment', 'blog.comment-validation')
snap('blog-comment-required')
fill_tab('txtComment', 'A comment from the parity test.')
click('btnComment', 'blog.comment')
snap('blog-commented')
goto('/news/9-new-online-store-is-open.aspx')
click('btnComment', 'news.comment-validation')
snap('news-comment-required')
fill('txtTitle', 'Parity')
fill_tab('txtComment', 'A news comment from the parity test.')
click('btnComment', 'news.comment')
snap('news-commented')
sql('comments', 'SELECT BlogPostID AS ItemID, CommentText AS Text, IPAddress FROM Nop_BlogComment UNION ALL SELECT NewsID, Title + \': \' + Comment, IPAddress FROM Nop_NewsComment', 'blog.comment,news.comment')
goto('/register.aspx')
click('ctrlPoll_btnSubmitVoteRecord', 'poll.validation')
snap('poll-required')
s('check', target='ctrlPoll_rblPollAnswers_1')
click('ctrlPoll_btnSubmitVoteRecord', 'poll.vote')
snap('poll-voted')
sql('poll-answers', 'SELECT PollAnswerID, Name, Count FROM Nop_PollAnswer ORDER BY PollAnswerID', 'poll.vote')

ADMIN_LOGIN = 'admin@yourstore.com'

# ---------------------------------------------------------------------------------------------------------------------
phase('6. The administration: its login, the customer refused, every page of it')
# The customer (logged in again: the wishlist's new browser) is not an administrator.
goto('/login.aspx')
fill('LoginForm_UserName', 'parity@example.com')
fill('LoginForm_Password', 'Parity-1')
click('LoginButton')
goto('/administration/default.aspx', 'admin-as-customer', 'admin.access-denied')
s('newcontext')
goto('/administration/default.aspx', 'admin-anonymous', 'admin.login-required')
goto('/administration/login.aspx', 'admin-login-page', 'admin.login-page')
click('LoginButton', 'admin.login-validation')
snap('admin-login-required')
fill('LoginForm_UserName', ADMIN_LOGIN)
fill_tab('LoginForm_Password', 'wrong')
click('LoginButton', 'admin.login-wrong')
snap('admin-login-wrong')
fill('LoginForm_UserName', ADMIN_LOGIN)
fill_tab('LoginForm_Password', 'admin')
click('LoginButton', 'admin.login')
snap('admin-logged-in')
s('cookies', 'admin.login', name='cookies-admin')
for path, name in ADMIN_PAGES:
    # (RecurringPaymentDetails of an unknown id redirects to /RecurringPayments.aspx: not the administration's folder, IIS's 404)
    goto(path, 'admin-' + name, 'admin.page.' + name, error=name.endswith('-none'))

def crud(key, add, fields, name_field, table, name_column, delete=True, selects=None, checks=None, extra_where=None, op_prefix='admin.', checks_first=None, query=None):
    """An entity of the administration: its add page's validation (Save with nothing filled), the entity added (Save and stay:
    its details page), edited (its name; Save and stay), saved to the list (Save), its details again (the browser's back),
    deleted (Delete, the confirmation's Yes); the database's rows of it after each change."""
    op = f'{op_prefix}{key}'
    where = f"{name_column} LIKE 'Parity%'" + (f' OR {extra_where}' if extra_where else '')
    query = query or f'SELECT * FROM {table} WHERE {where} ORDER BY 1'
    goto(add)
    click('SaveButton', f'{op}-validation')
    snap(f'{key}-add-required')
    for target in (checks_first or []):
        s('check', target=target)
    for target, value in fields:
        fill(target, value)
    for target, value in (selects or []):
        s('select', target=target, value=value)
    for target in (checks or []):
        s('check', target=target)
    click('SaveAndStayButton', f'{op}-add')
    snap(f'{key}-added')
    sql(f'{key}-rows-added', query, f'{op}-add')
    fill(name_field, f'Parity {key} edited')
    click('SaveAndStayButton', f'{op}-edit')
    snap(f'{key}-edited')
    click('SaveButton', f'{op}-edit')
    snap(f'{key}-list')
    sql(f'{key}-rows-edited', query, f'{op}-edit')
    if delete:
        s('back')
        click('DeleteButton', f'{op}-delete')
        click('btnYES', f'{op}-delete')
        snap(f'{key}-deleted')
        sql(f'{key}-rows-deleted', query, f'{op}-delete')


A = '/administration/'
# ---------------------------------------------------------------------------------------------------------------------
phase('7. The administration: the entities added, edited, deleted')
crud('manufacturer', A + 'ManufacturerAdd.aspx', [('ctrlManufacturerInfo_txtName_txtValue', 'Parity Manufacturer'), ('ctrlManufacturerInfo_txtDisplayOrder_txtValue', '5')],
     'ctrlManufacturerInfo_txtName_txtValue', 'Nop_Manufacturer', 'Name')
crud('category', A + 'CategoryAdd.aspx', [('ctrlCategoryInfo_txtName_txtValue', 'Parity Category'), ('ctrlCategoryInfo_txtDisplayOrder_txtValue', '5')],
     'ctrlCategoryInfo_txtName_txtValue', 'Nop_Category', 'Name', selects=[('ctrlCategoryInfo_ParentCategory_ddlCategories', '30')])
crud('product-attribute', A + 'ProductAttributeAdd.aspx', [('ctrlProductAttributeInfo_txtName_txtValue', 'Parity Attribute')],
     'ctrlProductAttributeInfo_txtName_txtValue', 'Nop_ProductAttribute', 'Name')
crud('specification-attribute', A + 'SpecificationAttributeAdd.aspx', [('ctrlSpecificationAttributeInfo_txtName_txtValue', 'Parity Specification'),
     ('ctrlSpecificationAttributeInfo_txtDisplayOrder_txtValue', '3')], 'ctrlSpecificationAttributeInfo_txtName_txtValue', 'Nop_SpecificationAttribute', 'Name')
crud('checkout-attribute', A + 'CheckoutAttributeAdd.aspx', [('ctrlCheckoutAttributeInfo_txtName_txtValue', 'Parity Gift Wrap'),
     ('ctrlCheckoutAttributeInfo_txtDisplayOrder_txtValue', '1')], 'ctrlCheckoutAttributeInfo_txtName_txtValue', 'Nop_CheckoutAttribute', 'Name')
crud('product-template', A + 'ProductTemplateAdd.aspx', [('ctrlProductTemplateInfo_txtName_txtValue', 'Parity Template'),
     ('ctrlProductTemplateInfo_txtTemplatePath', 'Templates\\Products\\OneVariant.ascx'), ('ctrlProductTemplateInfo_txtDisplayOrder_txtValue', '9')],
     'ctrlProductTemplateInfo_txtName_txtValue', 'Nop_ProductTemplate', 'Name')
crud('category-template', A + 'CategoryTemplateAdd.aspx', [('ctrlCategoryTemplateInfo_txtName_txtValue', 'Parity Template'),
     ('ctrlCategoryTemplateInfo_txtTemplatePath', '~\\Templates\\Categories\\ProductsInGrid.ascx'), ('ctrlCategoryTemplateInfo_txtDisplayOrder_txtValue', '9')],
     'ctrlCategoryTemplateInfo_txtName_txtValue', 'Nop_CategoryTemplate', 'Name')
crud('manufacturer-template', A + 'ManufacturerTemplateAdd.aspx', [('ctrlManufacturerTemplateInfo_txtName_txtValue', 'Parity Template'),
     ('ctrlManufacturerTemplateInfo_txtTemplatePath', '~\\Templates\\Manufacturers\\ProductsInGrid.ascx'), ('ctrlManufacturerTemplateInfo_txtDisplayOrder_txtValue', '9')],
     'ctrlManufacturerTemplateInfo_txtName_txtValue', 'Nop_ManufacturerTemplate', 'Name')
crud('customer-role', A + 'CustomerRoleAdd.aspx', [('ctrlCustomerRoleInfo_txtName_txtValue', 'Parity Role')],
     'ctrlCustomerRoleInfo_txtName_txtValue', 'Nop_CustomerRole', 'Name', checks=['ctrlCustomerRoleInfo_cbActive'])
crud('affiliate', A + 'AffiliateAdd.aspx', [('ctrlAffiliateInfo_txtFirstName', 'Parity'), ('ctrlAffiliateInfo_txtLastName_txtValue', 'Parity Affiliate'),
     ('ctrlAffiliateInfo_txtEmail_txtValue', 'affiliate@example.com'), ('ctrlAffiliateInfo_txtAddress1_txtValue', '1 Affiliate Way'),
     ('ctrlAffiliateInfo_txtCity_txtValue', 'Denver'), ('ctrlAffiliateInfo_txtZipPostalCode_txtValue', '80201')],
     'ctrlAffiliateInfo_txtLastName_txtValue', 'Nop_Affiliate', 'LastName', checks=['ctrlAffiliateInfo_cbActive'])
crud('campaign', A + 'CampaignAdd.aspx', [('ctrlCampaignInfo_txtName_txtValue', 'Parity Campaign'), ('ctrlCampaignInfo_txtSubject_txtValue', 'Parity news')],
     'ctrlCampaignInfo_txtName_txtValue', 'Nop_Campaign', 'Name')
crud('discount', A + 'DiscountAdd.aspx', [('pnlDiscountInfo_txtName_txtValue', 'Parity Discount'), ('pnlDiscountInfo_txtDiscountAmount_txtValue', '5'),
     ('pnlDiscountInfo_txtCouponCode', 'PARITY')], 'pnlDiscountInfo_txtName_txtValue', 'Nop_Discount', 'Name',
     checks_first=['pnlDiscountInfo_cbRequiresCouponCode'], delete=False)
crud('poll', A + 'PollAdd.aspx', [('ctrlPollInfo_txtName_txtValue', 'Parity Poll'), ('ctrlPollInfo_txtDisplayOrder_txtValue', '1')],
     'ctrlPollInfo_txtName_txtValue', 'Nop_Poll', 'Name', checks=['ctrlPollInfo_cbPublished'])
crud('news', A + 'NewsAdd.aspx', [('ctrlNewsInfo_txtTitle_txtValue', 'Parity News'), ('ctrlNewsInfo_txtShort', 'Short news of the parity test.')],
     'ctrlNewsInfo_txtTitle_txtValue', 'Nop_News', 'Title', checks=['ctrlNewsInfo_cbPublished', 'ctrlNewsInfo_cbAllowComments'])
crud('blog-post', A + 'BlogPostAdd.aspx', [('ctrlBlogPostInfo_txtBlogPostTitle_txtValue', 'Parity Post'), ('ctrlBlogPostInfo_txtTags', 'parity, test')],
     'ctrlBlogPostInfo_txtBlogPostTitle_txtValue', 'Nop_BlogPost', 'BlogPostTitle', checks=['ctrlBlogPostInfo_cbBlogPostAllowComments'])
crud('topic', A + 'TopicAdd.aspx', [('pnlInfo_txtSystemName_txtValue', 'ParityTopic'), ('rptrLanguageDivs_ctl00_txtTitle', 'Parity Topic')],
     'pnlInfo_txtSystemName_txtValue', 'Nop_Topic', 'Name', checks=['TopicTabs_pnlInfo_cbIncludeInSitemap'])
crud('forum-group', A + 'ForumGroupAdd.aspx', [('ctrlForumGroupInfo_txtName_txtValue', 'Parity Group'), ('ctrlForumGroupInfo_txtDisplayOrder_txtValue', '2')],
     'ctrlForumGroupInfo_txtName_txtValue', 'Nop_Forums_Group', 'Name')
crud('forum', A + 'ForumAdd.aspx', [('ctrlForumInfo_txtName_txtValue', 'Parity Forum'), ('ctrlForumInfo_txtDescription', 'A forum of the parity test.'),
     ('ctrlForumInfo_txtDisplayOrder_txtValue', '9')], 'ctrlForumInfo_txtName_txtValue', 'Nop_Forums_Forum', 'Name')
crud('email-account', A + 'EmailAccountAdd.aspx', [('ctrlEmailAccountInfo_txtEmailAddress_txtValue', 'parity@example.com'), ('ctrlEmailAccountInfo_txtDisplayName', 'Parity Account'),
     ('ctrlEmailAccountInfo_txtHost', '127.0.0.1'), ('ctrlEmailAccountInfo_txtPort', '9')], 'ctrlEmailAccountInfo_txtDisplayName', 'Nop_EmailAccount', 'DisplayName')
crud('blacklist-ip', A + 'BlacklistIPAdd.aspx', [('ctrlBlacklist_txtBannedIP', '10.20.30.40'), ('ctrlBlacklist_txtComment', 'Parity ban')],
     'ctrlBlacklist_txtComment', 'Nop_BannedIpAddress', 'Comment')
crud('blacklist-network', A + 'BlacklistNetworkAdd.aspx', [('ctrlBlacklist_txtBannedIP', '10.20.0.0-10.20.255.255'), ('ctrlBlacklist_txtComment', 'Parity network')],
     'ctrlBlacklist_txtComment', 'Nop_BannedIpNetwork', 'Comment')
crud('credit-card-type', A + 'CreditCardTypeAdd.aspx', [('ctrlCreditCardTypeInfo_txtName_txtValue', 'Parity Card'), ('ctrlCreditCardTypeInfo_txtSystemKeyword_txtValue', 'ParityCard')],
     'ctrlCreditCardTypeInfo_txtName_txtValue', 'Nop_CreditCardType', 'Name')
crud('tax-category', A + 'TaxCategoryAdd.aspx', [('ctrlTaxCategoryInfo_txtName_txtValue', 'Parity Tax'), ('ctrlTaxCategoryInfo_txtDisplayOrder_txtValue', '9')],
     'ctrlTaxCategoryInfo_txtName_txtValue', 'Nop_TaxCategory', 'Name')
crud('shipping-method', A + 'ShippingMethodAdd.aspx', [('ctrlShippingMethodInfo_txtName_txtValue', 'Parity Courier'), ('ctrlShippingMethodInfo_txtDisplayOrder_txtValue', '9')],
     'ctrlShippingMethodInfo_txtName_txtValue', 'Nop_ShippingMethod', 'Name')
crud('country', A + 'CountryAdd.aspx', [('ctrlCountryInfo_txtName_txtValue', 'Parityland'), ('ctrlCountryInfo_txtTwoLetterISOCode_txtValue', 'PL'),
     ('ctrlCountryInfo_txtThreeLetterISOCode_txtValue', 'PLD'), ('ctrlCountryInfo_txtNumericISOCode_txtValue', '999'), ('ctrlCountryInfo_txtDisplayOrder_txtValue', '999')],
     'ctrlCountryInfo_txtName_txtValue', 'Nop_Country', 'Name', checks=['ctrlCountryInfo_cbPublished'])
crud('state-province', A + 'StateProvinceAdd.aspx', [('ctrlStateProvinceInfo_txtName_txtValue', 'Parity State'), ('ctrlStateProvinceInfo_txtAbbreviation', 'PS'),
     ('ctrlStateProvinceInfo_txtDisplayOrder_txtValue', '1')], 'ctrlStateProvinceInfo_txtName_txtValue', 'Nop_StateProvince', 'Name')
crud('currency', A + 'CurrencyAdd.aspx', [('ctrlCurrencyInfo_txtName_txtValue', 'Parity Dollar'), ('ctrlCurrencyInfo_txtCurrencyCode_txtValue', 'PRT'),
     ('ctrlCurrencyInfo_txtRate_txtValue', '2.5'), ('ctrlCurrencyInfo_txtDisplayOrder_txtValue', '9')], 'ctrlCurrencyInfo_txtName_txtValue', 'Nop_Currency', 'Name',
     selects=[('ctrlCurrencyInfo_ddlDisplayLocale', 'en-US')])
crud('language', A + 'LanguageAdd.aspx', [('ctrlLanguageInfo_txtName_txtValue', 'Parity German'), ('ctrlLanguageInfo_txtDisplayOrder_txtValue', '9')],
     'ctrlLanguageInfo_txtName_txtValue', 'Nop_Language', 'Name', selects=[('ctrlLanguageInfo_ddlLanguageCulture', 'de-DE')])
crud('warehouse', A + 'WarehouseAdd.aspx', [('ctrlWarehouseInfo_txtName_txtValue', 'Parity Warehouse'), ('ctrlWarehouseInfo_txtCity', 'Reno')],
     'ctrlWarehouseInfo_txtName_txtValue', 'Nop_Warehouse', 'Name')
crud('measure-dimension', A + 'MeasureDimensionAdd.aspx', [('ctrlMeasureDimensionInfo_txtName_txtValue', 'Parity metre'), ('ctrlMeasureDimensionInfo_txtSystemKeyword', 'paritymetre'),
     ('ctrlMeasureDimensionInfo_txtRatio_txtValue', '0.0254'), ('ctrlMeasureDimensionInfo_txtDisplayOrder_txtValue', '9')],
     'ctrlMeasureDimensionInfo_txtName_txtValue', 'Nop_MeasureDimension', 'Name')
crud('measure-weight', A + 'MeasureWeightAdd.aspx', [('ctrlMeasureWeightInfo_txtName_txtValue', 'Parity gram'), ('ctrlMeasureWeightInfo_txtSystemKeyword', 'paritygram'),
     ('ctrlMeasureWeightInfo_txtRatio_txtValue', '453.59'), ('ctrlMeasureWeightInfo_txtDisplayOrder_txtValue', '9')],
     'ctrlMeasureWeightInfo_txtName_txtValue', 'Nop_MeasureWeight', 'Name')
crud('setting', A + 'SettingAdd.aspx', [('ctrlSettingInfo_txtName_txtValue', 'Parity.Setting'), ('ctrlSettingInfo_txtValue', 'parity'), ('ctrlSettingInfo_txtDescription', 'A setting of the parity test')],
     'ctrlSettingInfo_txtName_txtValue', 'Nop_Setting', 'Name')
crud('pricelist', A + 'PricelistAdd.aspx', [('pnlPricelistInfo_txtDisplayName_txtValue', 'Parity Pricelist'), ('PricelistsTabs_pnlPricelistInfo_txtShortName', 'parity'),
     ('pnlPricelistInfo_txtBody_txtValue', '{Name};{Price}'), ('pnlPricelistInfo_txtCacheTime_txtValue', '1'), ('pnlPricelistInfo_txtPriceAdjustment_txtValue', '0')],
     'pnlPricelistInfo_txtDisplayName_txtValue', 'Nop_Pricelist', 'DisplayName')
crud('payment-method', A + 'PaymentMethodAdd.aspx', [('txtName_txtValue', 'Parity Payment'), ('txtVisibleName_txtValue', 'Parity Pay'),
     ('txtClassName_txtValue', 'NopSolutions.NopCommerce.Payment.Methods.Check.CheckPaymentProcessor, Nop.Payment.Check'), ('txtDisplayOrder_txtValue', '99')],
     'txtName_txtValue', 'Nop_PaymentMethod', 'Name')
crud('shipping-rate-method', A + 'ShippingRateComputationMethodAdd.aspx', [('txtName_txtValue', 'Parity Rates'),
     ('txtClassName_txtValue', 'NopSolutions.NopCommerce.Shipping.Methods.FixedRateShipping.FixedRateShippingComputationMethod, Nop.Shipping.FixedRateShipping'), ('txtDisplayOrder_txtValue', '99')],
     'txtName_txtValue', 'Nop_ShippingRateComputationMethod', 'Name')
crud('tax-provider', A + 'TaxProviderAdd.aspx', [('txtName_txtValue', 'Parity Tax Provider'),
     ('txtClassName_txtValue', 'NopSolutions.NopCommerce.Tax.FixedRateTaxProvider, Nop.Tax.FixedRateTaxProvider'), ('txtDisplayOrder_txtValue', '99')],
     'txtName_txtValue', 'Nop_TaxProvider', 'Name')

def png(width=40, height=30):
    """A small PNG made here (the same bytes every time): a gradient, for the uploads (resized by the application)."""
    import zlib, struct, base64
    rows = b''.join(b'\x00' + bytes(v for x in range(width) for v in ((x * 6) % 256, (y * 8) % 256, ((x + y) * 3) % 256)) for y in range(height))
    def chunk(kind, data):
        return struct.pack('>I', len(data)) + kind + data + struct.pack('>I', zlib.crc32(kind + data) & 0xffffffff)
    data = b'\x89PNG\r\n\x1a\n' + chunk(b'IHDR', struct.pack('>IIBBBBB', width, height, 8, 2, 0, 0, 0)) + chunk(b'IDAT', zlib.compress(rows, 9)) + chunk(b'IEND', b'')
    return 'base64:' + base64.b64encode(data).decode()


def jpeg():
    """A small JPEG (the sample's default avatar, 901 bytes: an avatar must be a GIF or JPEG of at most 20 KB)."""
    import base64
    with open(os.path.join(os.path.dirname(os.path.abspath(__file__)), '..', 'work', 'nopCommerce-release-1.90', 'NopCommerceStore', 'images', 'defaultAvatar.jpg'), 'rb') as f:
        return 'base64:' + base64.b64encode(f.read()).decode()


PRODUCTS = ("SELECT p.ProductID, p.Name, p.ShortDescription, p.Published, p.Deleted, p.TemplateID, v.ProductVariantID, v.Name AS Variant, v.SKU, v.Price, v.OldPrice, "
            "v.StockQuantity, v.Published AS VariantPublished, v.Deleted AS VariantDeleted FROM Nop_Product p LEFT JOIN Nop_ProductVariant v ON v.ProductID = p.ProductID "
            "WHERE p.Name LIKE 'Parity%' ORDER BY p.ProductID, v.ProductVariantID")
PRODUCT_LINKS = ("SELECT 'category' AS Kind, m.ProductID, m.CategoryID AS Other FROM Nop_Product_Category_Mapping m JOIN Nop_Product p ON p.ProductID = m.ProductID WHERE p.Name LIKE 'Parity%' "
                 "UNION ALL SELECT 'manufacturer', m.ProductID, m.ManufacturerID FROM Nop_Product_Manufacturer_Mapping m JOIN Nop_Product p ON p.ProductID = m.ProductID WHERE p.Name LIKE 'Parity%' "
                 "UNION ALL SELECT 'related', r.ProductID1, r.ProductID2 FROM Nop_RelatedProduct r JOIN Nop_Product p ON p.ProductID = r.ProductID1 WHERE p.Name LIKE 'Parity%' "
                 "UNION ALL SELECT 'cross-sell', c.ProductID1, c.ProductID2 FROM Nop_CrossSellProduct c JOIN Nop_Product p ON p.ProductID = c.ProductID1 WHERE p.Name LIKE 'Parity%' "
                 "UNION ALL SELECT 'picture', pp.ProductID, pp.PictureID FROM Nop_ProductPicture pp JOIN Nop_Product p ON p.ProductID = pp.ProductID WHERE p.Name LIKE 'Parity%' "
                 "UNION ALL SELECT 'specification', s.ProductID, s.SpecificationAttributeOptionID FROM Nop_Product_SpecificationAttribute_Mapping s JOIN Nop_Product p ON p.ProductID = s.ProductID WHERE p.Name LIKE 'Parity%' "
                 "UNION ALL SELECT 'tag', t.ProductID, t.ProductTagID FROM Nop_ProductTag_Product_Mapping t JOIN Nop_Product p ON p.ProductID = t.ProductID WHERE p.Name LIKE 'Parity%' "
                 "ORDER BY 1, 2, 3")
PICTURES = "SELECT p.PictureID, p.MimeType, DATALENGTH(p.PictureBinary) AS Bytes, p.IsNew FROM Nop_Picture p WHERE p.PictureID > 160 ORDER BY 1"
VARIANT_DETAILS = ("SELECT 'tier' AS Kind, t.ProductVariantID, t.Quantity AS A, CONVERT(nvarchar(50), t.Price) AS B FROM Nop_TierPrice t WHERE t.ProductVariantID = 76 "
                   "UNION ALL SELECT 'role-price', r.ProductVariantID, r.CustomerRoleID, CONVERT(nvarchar(50), r.Price) FROM Nop_CustomerRole_ProductPrice r WHERE r.ProductVariantID = 76 "
                   "UNION ALL SELECT 'attribute', m.ProductVariantID, m.ProductAttributeID, m.TextPrompt FROM Nop_ProductVariant_ProductAttribute_Mapping m WHERE m.ProductVariantID = 76 "
                   "UNION ALL SELECT 'value', m.ProductVariantID, v.ProductVariantAttributeID, v.Name + ' ' + CONVERT(nvarchar(50), v.PriceAdjustment) FROM Nop_ProductVariantAttributeValue v JOIN Nop_ProductVariant_ProductAttribute_Mapping m ON m.ProductVariantAttributeID = v.ProductVariantAttributeID WHERE m.ProductVariantID = 76 "
                   "ORDER BY 1, 2, 3, 4")
PD = 'ctrlProductDetails_ProductTabs_'

# ---------------------------------------------------------------------------------------------------------------------
phase('8. The administration: products (added, edited, pictures, specifications, links, variants, copied, deleted), reviews, tags, bulk edit')
goto(A + 'ProductAdd.aspx')
click('SaveButton', 'admin.product-validation')
snap('product-add-required')
fill('ctrlProductInfoAdd_txtName_txtValue', 'Parity Product')
fill('ctrlProductInfoAdd_txtShortDescription', 'A product of the parity test')
fill('ctrlProductInfoAdd_txtSKU', 'PARITY-1')
fill('ctrlProductInfoAdd_txtPrice_txtValue', '12.34')
fill('ctrlProductInfoAdd_txtProductTags', 'parity, test')
fill('ctrlProductInfoAdd_txtStockQuantity_txtValue', '50')
tab('pnlCategoryMappings')
s('check', target='gvCategoryMappings_ctl03_cbCategoryInfo')
tab('pnlManufacturerMappings')
s('check', target='gvManufacturerMappings_ctl03_cbManufacturerInfo')
click('SaveAndStayButton', 'admin.product-add')
snap('product-added')
sql('product-rows-added', PRODUCTS, 'admin.product-add')
sql('product-links-added', PRODUCT_LINKS, 'admin.product-add')
fill(PD + 'pnlProductInfo_ctrlProductInfoEdit_txtName_txtValue', 'Parity Product edited')
click('SaveAndStayButton', 'admin.product-edit')
snap('product-edited')
# A picture uploaded (stored in the database: Media.Images.StoreInDB), its thumbnail made when shown.
tab('pnlPictures', 'admin.product-tabs')
s('upload', target='fuProductPicture1', value='parity.png:' + png())
click('btnUploadProductPicture', 'admin.product-picture-upload')
snap('product-picture-uploaded')
sql('product-pictures', PICTURES, 'admin.product-picture-upload')
# A specification attribute's option (Screensize) added to the product.
tab('pnlProductSpecification')
click('btnNewProductSpecification', 'admin.product-specification-add')
snap('product-specification-added')
sql('product-links-edited', PRODUCT_LINKS, 'admin.product-picture-upload,admin.product-specification-add')
# The product on the storefront: its page, its picture (resized), the category with it.
s('newcontext')
goto('/products/96-parity-product-edited.aspx', 'storefront-parity-product', 'admin.product-storefront')
s('assets', 'admin.product-storefront', name='storefront-parity-product-assets')
goto('/category/29-books.aspx', 'storefront-category-with-parity', 'admin.product-storefront')
goto('/administration/login.aspx')
fill('LoginForm_UserName', ADMIN_LOGIN)
fill('LoginForm_Password', 'admin')
click('LoginButton')
# Related products and cross-sells: the search popup (a window), a product chosen.
goto(A + 'RelatedProductAdd.aspx?pid=96&BtnId=x')
fill('ctrlRelatedProductAdd_txtProductName', 'Diamond')
click('ctrlRelatedProductAdd_SearchButton', 'admin.product-related-search')
snap('related-product-search')
s('script', value="const c = document.querySelector(\"input[type=checkbox][id*='cbProductInfo']\"); if (c) c.checked = true;")
s('click', 'admin.product-related-add', target="css=input[type=submit][id$='SaveButton'], input[type=submit][id$='btnSave']")
snap('related-product-added')
goto(A + 'CrossSellProductAdd.aspx?pid=96&BtnId=x')
fill('ctrlCrossSellProductAdd_txtProductName', 'Cooking')
click('ctrlCrossSellProductAdd_SearchButton', 'admin.product-cross-sell-search')
snap('cross-sell-product-search')
s('script', value="const c = document.querySelector(\"input[type=checkbox][id*='cbProductInfo']\"); if (c) c.checked = true;")
s('click', 'admin.product-cross-sell-add', target="css=input[type=submit][id$='SaveButton'], input[type=submit][id$='btnSave']")
snap('cross-sell-product-added')
sql('product-links-related', PRODUCT_LINKS, 'admin.product-related-add,admin.product-cross-sell-add')
# The product's variant: tier prices, prices by customer role, attributes and their values.
goto(A + 'ProductVariantDetails.aspx?ProductVariantID=76', 'variant-details', 'admin.variant-details')
fill('pnlProductVariantInfo_ctrlProductVariantInfo_txtSKU', 'PARITY-VARIANT')
click('SaveAndStayButton', 'admin.variant-edit')
snap('variant-edited')
tab('pnlTierPrices')
fill('ctrlProductVariantTierPrices_txtNewQuantity_txtValue', '5')
fill('ctrlProductVariantTierPrices_txtNewPrice_txtValue', '1100')
click('btnNewTierPrice', 'admin.variant-tier-price')
snap('variant-tier-price-added')
tab('pnlCustomerRolePrices')
fill('ctrlProductPricesByCustomerRole_txtNewPrice_txtValue', '1000')
click('btnNewPrice', 'admin.variant-role-price')
snap('variant-role-price-added')
tab('pnlProductAttributes')
s('select', target='pnlAttr_ddlNewProductAttributes', value='5')
fill('pnlAttr_txtNewTextPrompt', 'Parity prompt')
click('btnNewProductVariantAttribute', 'admin.variant-attribute-add')
snap('variant-attribute-added')
sql('variant-details-rows', VARIANT_DETAILS, 'admin.variant-tier-price,admin.variant-role-price,admin.variant-attribute-add')
goto(A + 'ProductVariantAttributeValues.aspx?ProductVariantAttributeId=22', 'variant-attribute-values', 'admin.variant-attribute-values')
fill('txtNewName_txtValue', 'Parity CPU')
fill('txtNewPriceAdjustment_txtValue', '7')
click('ctrlProductVariantAttributeValues_btnAdd', 'admin.variant-attribute-value-add')
snap('variant-attribute-value-added')
sql('variant-details-values', VARIANT_DETAILS, 'admin.variant-attribute-value-add')
goto(A + 'ProductVariantAdd.aspx?ProductId=96')
fill('txtName', 'Parity Variant')
fill('txtPrice_txtValue', '20')
click('SaveAndStayButton', 'admin.variant-add')
snap('variant-added')
sql('product-rows-variant', PRODUCTS, 'admin.variant-add')
# The product copied (the confirmation's dialog: its name), then the copy deleted; the product list searched; bulk edit.
goto(A + 'ProductDetails.aspx?ProductId=96')
click('btnDuplicate', 'admin.product-copy')
fill('txtProductCopyName', 'Parity Product copy')
click('btnDuplicateOk', 'admin.product-copy')
snap('product-copied')
sql('product-rows-copied', PRODUCTS, 'admin.product-copy')
click('DeleteButton', 'admin.product-delete')
click('btnYES', 'admin.product-delete')
snap('product-copy-deleted')
sql('product-rows-deleted', PRODUCTS, 'admin.product-delete')
goto(A + 'products.aspx')
fill('ctrlProducts_txtProductName', 'Parity')
click('ctrlProducts_SearchButton', 'admin.products-search')
snap('products-search')
fill('ctrlProducts_txtSKU_txtValue', 'PARITY-1')
click('btnGoDirectlyToSKU', 'admin.products-go-to-sku')
snap('products-go-to-sku')
goto(A + 'bulkeditproducts.aspx')
fill('ctrlBulkEditProducts_txtProductName', 'Parity')
click('ctrlBulkEditProducts_SearchButton', 'admin.bulk-edit-search')
snap('bulk-edit-search')
s('script', value="const c = document.querySelector(\"input[type=checkbox][id$='_cbProductVariant']\"); if (c) c.checked = true; const p = document.querySelector(\"input[id$='_txtPrice_txtValue']\"); if (p) p.value = '15.5';")
click('ctrlBulkEditProducts_btnUpdate', 'admin.bulk-edit-update')
snap('bulk-edit-updated')
sql('product-rows-bulk', PRODUCTS, 'admin.bulk-edit-update')
# Reviews: the customer's (pending) approved, edited; tags: one deleted.
goto(A + 'productreviews.aspx', 'product-reviews-list', 'admin.reviews')
goto(A + 'ProductReviewDetails.aspx?ProductReviewId=1')
s('check', target='cbIsApproved')
fill('txtTitle_txtValue', 'Parity review approved')
click('SaveButton', 'admin.review-approve')
snap('review-approved')
sql('reviews-approved', REVIEWS, 'admin.review-approve')
goto('/products/45-black-white-diamond-heart.aspx', 'product-with-review', 'admin.review-storefront')
goto(A + 'producttags.aspx')
s('check', target='gvProductTags_ctl02_cbProductTag')
click('ctrlProductTags_btnDelete', 'admin.tags-delete')
click('btnYES', 'admin.tags-delete')
snap('tags-deleted')
sql('tags', 'SELECT ProductTagID, Name, ProductCount FROM Nop_ProductTag ORDER BY ProductTagID', 'admin.tags-delete')
goto(A + 'productvariantslowstock.aspx', 'low-stock-after', 'admin.low-stock')

OD = 'ctrlOrderDetails_OrderTabs_'
GIFT_CARDS = 'SELECT GiftCardID, PurchasedOrderProductVariantID, Amount, IsGiftCardActivated, RecipientName, RecipientEmail, SenderName, IsRecipientNotified FROM Nop_GiftCard ORDER BY 1'
REWARD_POINTS = 'SELECT c.Email, h.Points, h.PointsBalance, h.UsedAmount, h.Message FROM Nop_RewardPointsHistory h JOIN Nop_Customer c ON c.CustomerID = h.CustomerID ORDER BY h.RewardPointsHistoryID'
RETURNS = 'SELECT r.OrderProductVariantID, r.Quantity, r.ReasonForReturn, r.RequestedAction, r.CustomerComments, r.StaffNotes, r.ReturnStatusID FROM Nop_ReturnRequest r ORDER BY r.ReturnRequestID'


def confirm(button, extender, op):
    """A button with a ConfirmButtonExtender and its modal popup: clicked, the popup's Yes."""
    click(button, op)
    click(f'{extender}_btnYES', op)


# ---------------------------------------------------------------------------------------------------------------------
phase('9. The administration: the order (paid, shipped, delivered, notes, totals, invoices), the customer, gift cards, returns, comments, e-mails, reports')
goto(A + 'OrderDetails.aspx?OrderId=1', 'order-1-details', 'admin.order-details')
s('download', 'admin.order-invoice-pdf', target='btnGetInvoicePDF', name='admin-order-invoice-pdf', content='pdf')
s('download', 'admin.order-packaging-slip-pdf', target='btnPrintPdfPackagingSlip', name='admin-order-packaging-slip-pdf', content='pdf')
confirm('btnMarkAsPaid', 'cbMarkAsPaid', 'admin.order-mark-paid')
snap('order-1-paid')
tab('pnlOrderShippingInfo', 'admin.order-tabs')
fill('txtTrackingNumber', 'PARITY-TRACK-1')
click('btnSetTrackingNumber', 'admin.order-tracking-number')
confirm('btnSetAsShipped', 'cbSetAsShipped', 'admin.order-shipped')
snap('order-1-shipped')
tab('pnlOrderShippingInfo')
confirm('btnSetAsDelivered', 'cbSetAsDelivered', 'admin.order-delivered')
snap('order-1-delivered')
tab('pnlOrderNotes')
fill('txtNewOrderNote', 'A note of the parity test')
s('check', target='cbNewDisplayToCustomer')
click('btnAddNewOrderNote', 'admin.order-note-add')
snap('order-1-note-added')
sql('orders-after-admin', ORDERS, 'admin.order-mark-paid,admin.order-shipped,admin.order-delivered')
sql('order-notes-after-admin', ORDER_NOTES, 'admin.order-note-add')
sql('gift-cards-after-paid', GIFT_CARDS, 'admin.order-mark-paid')
sql('reward-points-after-complete', REWARD_POINTS, 'admin.order-delivered')
goto(A + 'orders.aspx')
fill('ctrlOrders_txtCustomerEmail', 'parity@example.com')
click('ctrlOrders_SearchButton', 'admin.orders-search')
snap('orders-search')
fill('ctrlOrders_txtOrderId_txtValue', '1')
click('btnGoDirectlyToOrderNumber', 'admin.orders-go-to-number')
snap('orders-go-to-number')
# The gift card bought in the order: activated, the recipient notified (an e-mail queued).
goto(A + 'purchasedgiftcards.aspx', 'purchased-gift-cards-after', 'admin.gift-cards')
goto(A + 'GiftCardDetails.aspx?GiftCardId=1')
s('check', target='cbIsGiftCardActivated')
click('SaveAndStayButton', 'admin.gift-card-activate')
snap('gift-card-activated')
click('btnNotifyRecipient', 'admin.gift-card-notify')
snap('gift-card-notified')
sql('gift-cards-activated', GIFT_CARDS, 'admin.gift-card-activate,admin.gift-card-notify')
# The customer: edited, a role, reward points, an e-mail sent; the customers list searched; a new customer added and deleted.
goto(A + 'CustomerDetails.aspx?CustomerId=38', 'customer-38-details', 'admin.customer-details')
fill('ctrlCustomerInfo_txtAdminComment', 'Edited by the parity test')
click('SaveAndStayButton', 'admin.customer-comment')
snap('customer-38-edited')
tab('pnlCustomerRoleMappings', 'admin.customer-tabs')
snap('customer-38-roles')
tab('pnlCustomerRewardPoints')
fill('txtNewPoints_txtValue', '25')
fill('txtNewMessage', 'Parity bonus')
click('btnAddPoints', 'admin.customer-reward-points')
snap('customer-38-reward-points')
sql('reward-points-added', REWARD_POINTS, 'admin.customer-reward-points')
for panel in ('pnlCustomerBillingAddresses', 'pnlCustomerShippingAddresses', 'pnlCustomerOrders', 'pnlCustomerShoppingCart', 'pnlCustomerWishlist', 'pnlCustomerPlaceOrder'):
    tab(panel, 'admin.customer-tabs')
    snap(f'customer-38-{panel[11:].lower()}')
tab('pnlCustomerSendEmail')
fill('ctrlCustomerSendEmail_txtSubject', 'Hello from the parity test')
# (the body is FCKeditor's, left empty: the subject only)
click('ctrlCustomerSendEmail_btnSend', 'admin.customer-send-email')
snap('customer-38-email-sent')
sql('queued-email-admin', QUEUE, 'admin.customer-send-email,admin.gift-card-notify')
goto(A + 'customers.aspx')
fill('ctrlCustomers_txtEmail', 'parity')
click('ctrlCustomers_SearchButton', 'admin.customers-search')
snap('customers-search')
crud('customer', A + 'CustomerAdd.aspx', [('ctrlCustomerInfo_txtEmail_txtValue', 'parity-new@example.com'), ('ctrlCustomerInfo_txtPassword', 'Parity-9'),
     ('ctrlCustomerInfo_txtFirstName', 'Parity'), ('ctrlCustomerInfo_txtLastName', 'New')], 'ctrlCustomerInfo_txtLastName', 'Nop_Customer', 'Email',
     checks=['ctrlCustomerInfo_cbActive'], query="SELECT Email, Username, IsAdmin, IsGuest, Active, Deleted, AdminComment FROM Nop_Customer WHERE Email LIKE 'parity-new%'")
# Impersonation: the administrator places an order as the customer (the storefront as the customer), then ends it.
goto(A + 'CustomerDetails.aspx?CustomerId=38')
tab('pnlCustomerPlaceOrder')
click('btnPlaceOrder', 'admin.customer-impersonate')
snap('impersonated-storefront')
s('clicktext', 'admin.customer-impersonate-finish', value='Finish session')
snap('impersonation-finished')
# Returns: the customer asks (the order is complete now); the administrator handles the request.
s('newcontext')
goto('/login.aspx')
fill('LoginForm_UserName', 'parity@example.com')
fill('LoginForm_Password', 'Parity-1')
click('LoginButton')
goto('/orderdetails.aspx?orderid=1', 'order-details-complete', 'acct.order-details')
click('btnReturnItems', 'acct.return-items-page')
snap('return-items-page')
s('script', value="const q = document.querySelector(\"select[id$='_ddlQuantity']\"); if (q) { q.selectedIndex = 1; }")
fill('txtComments', 'Returned by the parity test')
click('btnSubmit', 'acct.return-items')
snap('return-items-submitted')
sql('return-requests', RETURNS, 'acct.return-items')
goto('/account.aspx')
tab('pnlReturnRequests', 'acct.return-requests')
snap('account-return-requests')
tab('pnlDP', 'acct.downloadable-products')
snap('account-downloads-after-paid')
tab('pnlRewardPoints', 'acct.reward-points')
snap('account-reward-points-after')
goto('/administration/login.aspx')
fill('LoginForm_UserName', ADMIN_LOGIN)
fill('LoginForm_Password', 'admin')
click('LoginButton')
goto(A + 'returnrequests.aspx', 'return-requests-list', 'admin.returns')
goto(A + 'ReturnRequestDetails.aspx?ReturnRequestId=1', 'return-request-details', 'admin.return-details')
s('select', target='ddlStatus', value='20')
fill('txtStaffNotes', 'Accepted by the parity test')
click('SaveButton', 'admin.return-edit')
snap('return-request-saved')
sql('return-requests-admin', RETURNS, 'admin.return-edit')
# Comments: a blog comment and a news comment edited, then deleted.
goto(A + 'blogcomments.aspx', 'blog-comments-list', 'admin.blog-comments')
click('btnEditBlogComment', 'admin.blog-comment-edit')
snap('blog-comment-edit-page')
goto(A + 'blogcomments.aspx')
click('btnDeleteBlogComment', 'admin.blog-comment-delete')
snap('blog-comment-deleted')
goto(A + 'newscomments.aspx', 'news-comments-list', 'admin.news-comments')
click('btnEditNewsComment', 'admin.news-comment-edit')
snap('news-comment-edit-page')
goto(A + 'newscomments.aspx')
click('btnDeleteNewsComment', 'admin.news-comment-delete')
snap('news-comment-deleted')
sql('comments-after-admin', 'SELECT COUNT(*) AS BlogComments, (SELECT COUNT(*) FROM Nop_NewsComment) AS NewsComments FROM Nop_BlogComment', 'admin.blog-comment-delete,admin.news-comment-delete')
# E-mails: the queue loaded, an e-mail opened, requeued, deleted; a message template, a resource edited; the newsletter list.
goto(A + 'messagequeue.aspx')
s('uncheck', target='cbLoadNotSentItemsOnly')
click('ctrlMessageQueue_LoadButton', 'admin.message-queue-load')
snap('message-queue-loaded')
goto(A + 'MessageQueueDetails.aspx?QueuedEmailId=1', 'message-queue-email-1', 'admin.message-queue-details')
click('RequeueButton', 'admin.message-queue-requeue')
snap('message-queue-requeued')
goto(A + 'MessageQueueDetails.aspx?QueuedEmailId=1')
click('DeleteButton', 'admin.message-queue-delete')
click('btnYES', 'admin.message-queue-delete')
snap('message-queue-deleted')
sql('queue-after-admin', QUEUE, 'admin.message-queue-requeue,admin.message-queue-delete')
goto(A + 'MessageTemplateDetails.aspx?MessageTemplateID=25')
fill('rptrLanguageDivs_ctl00_txtSubject', '%Store.Name%. New blog comment (parity)')
click('SaveAndStayButton', 'admin.message-template-edit')
snap('message-template-edited')
sql('message-template-25', "SELECT MessageTemplateID, LanguageID, Subject, IsActive FROM Nop_MessageTemplateLocalized WHERE MessageTemplateID = 25", 'admin.message-template-edit')
goto(A + 'LocaleStringResourceDetails.aspx?LocaleStringResourceID=6919')
fill('txtResourceValue', 'Account activation (parity)')
click('SaveButton', 'admin.resource-edit')
snap('resource-edited')
goto(A + 'localestringresources.aspx')
fill('txtResourceName', 'Account.Account')
click('btnSearch', 'admin.resources-search')
snap('resources-search')
goto(A + 'newslettersubscribers.aspx')
click('ctrlNewsletterSubscribers_SearchButton', 'admin.newsletter-search')
snap('newsletter-subscribers-search')
# Reports, the log, the activity log, maintenance.
goto(A + 'salesreport.aspx')
click('ctrlSalesReport_SearchButton', 'admin.sales-report')
snap('sales-report-run')
goto(A + 'customerreports.aspx')
click('btnSearchByOrderTotal', 'admin.customer-report')
snap('customer-report-run')
goto(A + 'default.aspx', 'dashboard-after-orders', 'admin.dashboard')
# The log searched by its message (the e-mails' sending - every minute, by a task - adds its entries as time goes: not compared):
# the blacklist's refused IP of phase 7; its details.
# The log's entries so far but the e-mails' sending failures (log type 2, MailError: their number is the task's timing's).
sql('log-entries', 'SELECT LogTypeID, Severity, Message FROM Nop_Log WHERE LogTypeID <> 2 ORDER BY LogID', 'admin.logs')
goto(A + 'logs.aspx')
fill('ctrlLogs_txtMessage', 'valid IP')
click('ctrlLogs_btnSearch', 'admin.logs-search')
snap('logs-search')
s('clicktext', 'admin.log-details', value='Details')
snap('log-details')
goto(A + 'activitylog.aspx')
click('ctrlActivityLog_SearchButton', 'admin.activity-log-search')
snap('activity-log-search')
sql('activity-log', 'SELECT t.SystemKeyword, a.Comment FROM Nop_ActivityLog a JOIN Nop_ActivityLogType t ON t.ActivityLogTypeID = a.ActivityLogTypeID ORDER BY a.ActivityLogID', 'admin.activity-log-search,admin.activity-log')
goto(A + 'currentshoppingcarts.aspx', 'current-shopping-carts-after', 'admin.current-carts')
goto(A + 'onlinecustomers.aspx', 'online-customers-after', 'admin.online-customers')

GS = 'ctrlGlobalSettings_CommonSettingsTabs_'
FORUM_ROWS = ("SELECT 'topic' AS Kind, t.TopicID AS ID, t.ForumID, c.Email, t.Subject AS Text, t.NumPosts, t.TopicTypeID FROM Nop_Forums_Topic t JOIN Nop_Customer c ON c.CustomerID = t.UserID "
              "UNION ALL SELECT 'post', p.PostID, p.TopicID, c.Email, p.Text, 0, 0 FROM Nop_Forums_Post p JOIN Nop_Customer c ON c.CustomerID = p.UserID "
              "UNION ALL SELECT 'subscription', s.SubscriptionID, s.ForumID, c.Email, CONVERT(nvarchar(20), s.TopicID), 0, 0 FROM Nop_Forums_Subscription s JOIN Nop_Customer c ON c.CustomerID = s.UserID "
              "ORDER BY 1, 2")
PMS = ("SELECT f.Email AS FromEmail, t.Email AS ToEmail, m.Subject, m.Text, m.IsRead, m.IsDeletedByAuthor, m.IsDeletedByRecipient FROM Nop_Forums_PrivateMessage m "
       "JOIN Nop_Customer f ON f.CustomerID = m.FromUserID JOIN Nop_Customer t ON t.CustomerID = m.ToUserID ORDER BY m.PrivateMessageID")
SETTINGS_CHANGED = ("SELECT Name, Value FROM Nop_Setting WHERE Name IN ('Forums.ForumsEnabled', 'Forums.CustomersAllowedToEditPosts', 'Forums.CustomersAllowedToDeletePosts', "
                    "'Forums.CustomersAllowedToManageSubscriptions', 'Customer.CustomersAllowedToUploadAvatars', 'Customer.AllowViewingProfiles', 'Customer.AllowPM', "
                    "'Checkout.AnonymousCheckoutAllowed', 'Checkout.UseOnePageCheckout', 'Checkout.TermsOfServiceEnabled', 'Common.AllowCustomerSelectTheme', "
                    "'Common.StoreClosed', 'Common.LoginCaptchaImageEnabled', 'Common.CustomerRegistrationType', 'Common.AllowAnonymousUsersToReviewProduct') ORDER BY Name")


def login(user, password, path='/login.aspx'):
    goto(path)
    fill('LoginForm_UserName', user)
    fill('LoginForm_Password', password)
    click('LoginButton')


def admin_login():
    s('newcontext')
    login(ADMIN_LOGIN, 'admin', '/administration/login.aspx')


# ---------------------------------------------------------------------------------------------------------------------
phase('10. Settings that turn features on, and the features on the storefront: forums, profiles, avatars, private messages, themes')
goto(A + 'forumssettings.aspx')
for box in ('cbForumsEnabled', 'cbCustomersAllowedToEditPosts', 'cbCustomersAllowedToDeletePosts', 'cbCustomersAllowedToManageSubscriptions'):
    s('check', target=box)
click('ctrlForumsSettings_btnSave', 'admin.settings-forums')
snap('forums-settings-saved')
goto(A + 'globalsettings.aspx')
tab('pnlCustomerProfiles', 'admin.global-settings-tabs')
for box in ('cbCustomersAllowedToUploadAvatars', 'cbAllowViewingProfiles', 'cbAllowPM'):
    s('check', target=GS + 'pnlCustomerProfiles_' + box)
tab('pnlSEODisplay')
s('check', target=GS + 'pnlSEODisplay_cbAllowCustomerSelectTheme')
tab('pnlProducts')
s('check', target=GS + 'pnlProducts_cbAllowAnonymousUsersToReviewProduct')
click('ctrlGlobalSettings_btnSave', 'admin.settings-global')
snap('global-settings-saved')
sql('settings-features-on', SETTINGS_CHANGED, 'admin.settings-forums,admin.settings-global')
# The forums (as the customer): the groups, a forum, a new topic (validation), a reply, a quote, an edit, watch, search, active
# discussions, the topic moved and deleted by the administrator.
s('newcontext')
login('parity@example.com', 'Parity-1')
goto('/Boards/default.aspx', 'boards', 'forum.groups')
goto('/boards/fg/2/general.aspx', 'forum-group', 'forum.group')
goto('/boards/f/2/new-products.aspx', 'forum', 'forum.forum')
goto('/Boards/TopicNew.aspx?ForumId=2')
click('btnSubmit', 'forum.topic-new-validation')
snap('forum-topic-new-required')
fill('txtTopicTitle', 'Parity topic')
fill_tab('txtTopicBodyBBCode', 'The first post of the [b]parity[/b] topic.')
click('btnSubmit', 'forum.topic-new')
snap('forum-topic-created')
sql('forum-rows-topic', FORUM_ROWS, 'forum.topic-new')
click('btnReply', 'forum.post-reply')
fill('txtTopicBodyBBCode', 'A reply of the parity test.')
click('btnSubmit', 'forum.post-reply')
snap('forum-reply-posted')
s('clicktext', 'forum.post-quote', value='Quote')
snap('forum-quote-page')
click('btnSubmit', 'forum.post-quote')
snap('forum-quote-posted')
s('clicktext', 'forum.post-edit', value='Edit')
snap('forum-edit-page')
fill('txtTopicBodyBBCode', 'The first post of the parity topic, edited.')
click('btnSubmit', 'forum.post-edit')
snap('forum-post-edited')
click('btnWatchTopic', 'forum.watch-topic')
snap('forum-topic-watched')
goto('/boards/f/2/new-products.aspx')
click('btnWatchForum', 'forum.watch-forum')
snap('forum-watched')
sql('forum-rows-posts', FORUM_ROWS, 'forum.post-reply,forum.post-quote,forum.post-edit,forum.watch-topic,forum.watch-forum')
goto('/Boards/Search.aspx?searchterms=parity', 'forum-search', 'forum.search')
goto('/Boards/Search.aspx')
fill('ctrlForumSearch_txtSearchTerm', 'reply')
s('check', target='ctrlForumSearch_cbAdvancedSearch')
click('ctrlForumSearch_btnSearch', 'forum.search-advanced')
snap('forum-search-advanced')
goto('/Boards/ActiveDiscussions.aspx', 'forum-active-discussions', 'forum.active-discussions')
goto('/account.aspx')
tab('pnlForumSubscriptions', 'acct.forum-subscriptions')
snap('account-forum-subscriptions')
# Avatars (upload, the profile page with it, removal); the profile page; private messages to the administrator.
tab('pnlAvatar', 'acct.avatar')
s('upload', target='fuAvatar', value='avatar.jpg:' + jpeg())
click('btnUploadAvatar', 'acct.avatar-upload')
snap('account-avatar-uploaded')
goto('/profile.aspx?UserId=38', 'profile-customer', 'acct.profile')
goto('/profile.aspx?UserId=37', 'profile-admin', 'acct.profile')
click('btnSendPM', 'pm.send-page')
snap('pm-send-page')
click('btnSend', 'pm.send-validation')
snap('pm-send-required')
fill('txtSubject', 'Parity question')
fill_tab('txtMessageBBCode', 'A private message of the parity test.')
click('btnSend', 'pm.send')
snap('pm-sent')
goto('/privatemessages.aspx', 'pm-customer-box', 'pm.inbox')
tab('pnlPrivateMessagesSentItems', 'pm.sent-items')
snap('pm-customer-sent-items')
goto('/account.aspx')
tab('pnlAvatar')
click('btnRemoveAvatar', 'acct.avatar-remove')
snap('account-avatar-removed')
sql('avatars', 'SELECT c.Email, c.AvatarID, (SELECT COUNT(*) FROM Nop_Picture p WHERE p.PictureID = c.AvatarID) AS Pictures FROM Nop_Customer c WHERE c.IsGuest = 0 ORDER BY c.CustomerID', 'acct.avatar-upload,acct.avatar-remove')
# The theme selector of the footer.
goto('/default.aspx')
s('select', 'page.theme-selector', target='ctrlStoreTheme_ddlTheme', value='nopClassic')
snap('theme-selected')
s('assets', 'page.theme-selector', name='theme-assets')
s('select', 'page.theme-selector', target='ctrlStoreTheme_ddlTheme', value='darkOrange')
snap('theme-selected-back')
s('cookies', 'page.theme-selector', name='cookies-theme')
# The administrator: the private message read, replied to, deleted; the topic moved, then deleted.
admin_login()
goto('/privatemessages.aspx', 'pm-admin-inbox', 'pm.inbox')
s('clicktext', 'pm.view', value='Parity question')
snap('pm-admin-view')
click('btnReply', 'pm.reply')
fill('txtMessageBBCode', 'The administrator answers.')
click('btnSend', 'pm.reply')
snap('pm-replied')
goto('/privatemessages.aspx')
s('script', value="const c = document.querySelector(\"input[type=checkbox][id$='_cbSelect']\"); if (c) c.checked = true;")
click('btnMarkAsUnread', 'pm.mark-unread')
snap('pm-marked-unread')
s('script', value="const c = document.querySelector(\"input[type=checkbox][id$='_cbSelect']\"); if (c) c.checked = true;")
click('btnDeleteSelected', 'pm.delete')
snap('pm-deleted')
sql('private-messages', PMS, 'pm.send,pm.reply,pm.mark-unread,pm.delete')
goto('/boards/f/2/new-products.aspx')
s('clicktext', 'forum.topic-open', value='Parity topic')
snap('forum-topic-as-admin')
click('btnMoveTopic', 'forum.topic-move')
snap('forum-move-page')
s('select', target='ddlForums', value='3')
click('btnSubmit', 'forum.topic-move')
snap('forum-topic-moved')
click('btnDelete', 'forum.topic-delete')
snap('forum-topic-deleted')
sql('forum-rows-final', FORUM_ROWS, 'forum.topic-move,forum.topic-delete')

def global_settings(op, *boxes, uncheck=(), select=None):
    """The global settings page: checkboxes (tab, id) checked / unchecked, a select (tab, id, value); saved."""
    goto(A + 'globalsettings.aspx')
    for panel, box in boxes:
        tab(panel)
        s('check', target=GS + panel + '_' + box)
    for panel, box in uncheck:
        tab(panel)
        s('uncheck', target=GS + panel + '_' + box)
    if select:
        panel, target, value = select
        tab(panel)
        s('select', target=GS + panel + '_' + target, value=value)
    click('ctrlGlobalSettings_btnSave', op)


# ---------------------------------------------------------------------------------------------------------------------
phase('11. Checkout settings: a guest\'s checkout, the one-page checkout, terms of service, the other payment methods; the store closed; captcha; registration modes')
admin_login()
global_settings('admin.settings-checkout', ('pnlGeneral', 'cbAnonymousCheckoutAllowed'), ('pnlGeneral', 'cbUseOnePageCheckout'), ('pnlGeneral', 'cbCheckoutTermsOfService'))
snap('checkout-settings-saved')
sql('settings-checkout', SETTINGS_CHANGED, 'admin.settings-checkout')
# A guest: the cart, terms of service (not accepted, accepted), checkout as a guest, the one-page checkout with a new address,
# purchase order payment.
s('newcontext')
goto('/products/60-the-best-skillet-recipes.aspx')
click('btnAddToCart', 'cart.add-one-variant')
click('btnCheckout', 'checkout.terms-required')
snap('checkout-terms-required')
s('check', target='cbTermsOfService')
click('btnCheckout', 'checkout.anonymous')
snap('checkout-login-or-guest')
click('btnCheckoutAsGuest', 'checkout.guest')
snap('checkout-onepage')
OP = 'ctrlCheckoutOnePage_'
fill('ctrlShippingAddress_txtFirstName_txtValue', 'Guest')
fill('ctrlShippingAddress_txtLastName_txtValue', 'Buyer')
fill('ctrlShippingAddress_txtPhoneNumber_txtValue', '5550199')
fill('ctrlShippingAddress_txtEmail_txtValue', 'guest@example.com')
fill('ctrlShippingAddress_txtAddress1_txtValue', '30 Guest Road')
fill('ctrlShippingAddress_txtCity_txtValue', 'Austin')
fill('ctrlShippingAddress_txtZipPostalCode_txtValue', '73301')
s('click', 'checkout.onepage-shipping-address', target="css=[id$='ctrlCheckoutShippingAddress_btnNextStep']")
snap('checkout-onepage-billing')
s('click', 'checkout.onepage-billing-address', target="css=[id$='btnTheSameAsShippingAddress']")
snap('checkout-onepage-shipping-method')
s('check', 'checkout.onepage-shipping-method', target='dlShippingOptions_ctl02_rdShippingOption')
s('click', 'checkout.onepage-shipping-method', target="css=[id$='ctrlCheckoutShippingMethod_btnNextStep']")
snap('checkout-onepage-payment-method')
s('check', 'checkout.payment-purchase-order', target='dlPaymentMethod_ctl01_rdPaymentMethod')
s('click', 'checkout.payment-purchase-order', target="css=[id$='ctrlCheckoutPaymentMethod_btnNextStep']")
snap('checkout-onepage-payment-info')
fill('txtPONumber', 'PO-PARITY-1')
s('click', 'checkout.onepage-payment-info', target="css=[id$='ctrlCheckoutPaymentInfo_btnNextStep']")
snap('checkout-onepage-confirm')
s('click', 'checkout.onepage-confirm', target="css=[id$='ctrlCheckoutConfirm_btnNextStep']")
snap('checkout-onepage-completed')
sql('orders-guest', ORDERS, 'checkout.guest,checkout.onepage-confirm')
# The customer: the other offline payment methods (cash on delivery, check, pay in store) in the one-page checkout; the credit
# card form's validation; the reward points used.
s('newcontext')
login('parity@example.com', 'Parity-1')
NEXT_IF_SHOWN = "const b = document.querySelector(\"[id$='STEP_btnNextStep']\"); if (b && b.offsetParent) b.click();"
for index, (method, op) in enumerate([('2', 'checkout.payment-cod'), ('3', 'checkout.payment-check'), ('4', 'checkout.payment-in-store')]):
    goto('/products/60-the-best-skillet-recipes.aspx')
    click('btnAddToCart')
    s('check', target='cbTermsOfService')
    click('btnCheckout', 'checkout.onepage')
    s('click', 'checkout.onepage-shipping-address', target="css=[id$='dlShippingAddresses_ctl00_btnSelect']")
    s('click', 'checkout.onepage-billing-address', target="css=[id$='dlBillingAddresses_ctl00_btnSelect']")
    s('check', target='dlShippingOptions_ctl00_rdShippingOption')
    s('click', 'checkout.onepage-shipping-method', target="css=[id$='ctrlCheckoutShippingMethod_btnNextStep']")
    if index == 0:
        s('check', 'checkout.reward-points', target='cbUseRewardPoints')
    s('check', op, target=f'dlPaymentMethod_ctl0{method}_rdPaymentMethod')
    s('click', op, target="css=[id$='ctrlCheckoutPaymentMethod_btnNextStep']")
    snap(f'checkout-onepage-{op[9:]}-info')
    # (the reward points pay the whole order: no payment info step then)
    script(None, NEXT_IF_SHOWN.replace('STEP', 'ctrlCheckoutPaymentInfo'), 'checkout.onepage-payment-info')
    s('click', 'checkout.onepage-confirm', target="css=[id$='ctrlCheckoutConfirm_btnNextStep']")
    snap(f'checkout-onepage-{op[9:]}-completed')
goto('/products/60-the-best-skillet-recipes.aspx')
click('btnAddToCart')
sql('orders-payments', ORDERS, 'checkout.payment-cod,checkout.payment-check,checkout.payment-in-store,checkout.reward-points')
sql('reward-points-used', REWARD_POINTS, 'checkout.reward-points')
goto('/checkoutonepage.aspx')
s('click', target="css=[id$='dlShippingAddresses_ctl00_btnSelect']")
s('click', target="css=[id$='dlBillingAddresses_ctl00_btnSelect']")
s('check', target='dlShippingOptions_ctl00_rdShippingOption')
s('click', target="css=[id$='ctrlCheckoutShippingMethod_btnNextStep']")
s('check', target='dlPaymentMethod_ctl00_rdPaymentMethod')
s('click', target="css=[id$='ctrlCheckoutPaymentMethod_btnNextStep']")
s('click', 'checkout.payment-card-validation', target="css=[id$='ctrlCheckoutPaymentInfo_btnNextStep']")
snap('checkout-card-required')
fill('creditCardName', 'Parity Customer')
fill_tab('creditCardNumber', '1234')
s('click', 'checkout.payment-card-validation', target="css=[id$='ctrlCheckoutPaymentInfo_btnNextStep']")
snap('checkout-card-invalid')
# The gift card bought in order 1 (activated by the administrator) redeemed in the cart; the 'PARITY' discount coupon.
s('sql', value="UPDATE Nop_GiftCard SET GiftCardCouponCode = 'PARITY-GIFT' WHERE GiftCardID = 1")
goto('/shoppingcart.aspx')
fill('txtGiftCardCouponCode', 'PARITY-GIFT')
click('btnApplyGiftCardsCouponCode', 'cart.gift-card-apply')
snap('cart-gift-card-applied')
fill('txtDiscountCouponCode', 'PARITY')
click('btnApplyDiscountCouponCode', 'cart.discount-apply')
snap('cart-discount-parity')
click('btnRemoveGC', 'cart.gift-card-remove')
snap('cart-gift-card-removed')
# The store closed: the storefront's page for it, the administration still reachable; opened again.
admin_login()
global_settings('admin.settings-store-closed', ('pnlGeneral', 'cbStoreClosed'))
s('newcontext')
fetch('/default.aspx', 'store-closed-home', 'page.store-closed', follow=True)
fetch('/StoreClosed.htm', 'store-closed-page', 'page.store-closed', hash=True)
admin_login()
goto(A + 'default.aspx', 'store-closed-admin', 'page.store-closed')
global_settings('admin.settings-store-closed', uncheck=[('pnlGeneral', 'cbStoreClosed')])
# Captcha on the login and registration pages (its image is random: a wrong answer only).
global_settings('admin.settings-captcha', ('pnlSecurity', 'cbEnableLoginCaptchaImage'), ('pnlSecurity', 'cbEnableRegisterCaptchaImage'))
s('newcontext')
goto('/login.aspx', 'login-captcha', 'acct.captcha')
fill('LoginForm_UserName', 'parity@example.com')
fill('LoginForm_Password', 'Parity-1')
fill('txtCode', 'WRONG')
click('LoginButton', 'acct.captcha-wrong')
snap('login-captcha-wrong')
goto('/register.aspx', 'register-captcha', 'acct.captcha')
admin_login()
global_settings('admin.settings-captcha', uncheck=[('pnlSecurity', 'cbEnableLoginCaptchaImage'), ('pnlSecurity', 'cbEnableRegisterCaptchaImage')])
# Registration modes: e-mail validation (the activation link: its token made a known one), admin approval, disabled; usernames.
global_settings('admin.settings-registration', ('pnlOther', 'cbUsernamesEnabled'), select=('pnlOther', 'ddlRegistrationMethod', '2'))
s('newcontext')
goto('/register.aspx')
fill(REG + 'txtFirstName', 'Activation')
fill(REG + 'txtLastName', 'Needed')
fill(REG + 'Email', 'activate@example.com')
fill(REG + 'UserName', 'parity-activate')
fill(REG + 'txtStreetAddress', '5 Activation Street')
fill(REG + 'txtZipPostalCode', '10021')
fill(REG + 'txtCity', 'New York')
fill(REG + 'txtPhoneNumber', '12345678')
fill(REG + 'Password', 'Parity-1')
fill(REG + 'ConfirmPassword', 'Parity-1')
click('StepNextButton', 'acct.register-email-validation')
snap('registered-needs-activation')
sql('queued-email-activation', QUEUE, 'acct.register-email-validation')
s('sql', value="UPDATE a SET Value = '00000000-0000-0000-0000-0000000000d1' FROM Nop_CustomerAttribute a JOIN Nop_Customer c ON c.CustomerID = a.CustomerId WHERE c.Email = 'activate@example.com' AND a.[Key] = 'AccountActivationToken'")
login('parity-activate', 'Parity-1')
snap('login-not-activated')
goto('/accountactivation.aspx?act=00000000-0000-0000-0000-0000000000ff&email=activate@example.com', 'account-activation-wrong', 'acct.activation')
goto('/accountactivation.aspx?act=00000000-0000-0000-0000-0000000000d1&email=activate@example.com', 'account-activated', 'acct.activation')
login('parity-activate', 'Parity-1')
snap('login-username-activated')
sql('customers-after-activation', CUSTOMERS, 'acct.activation')
admin_login()
global_settings('admin.settings-registration', select=('pnlOther', 'ddlRegistrationMethod', '4'))
s('newcontext')
goto('/register.aspx', 'register-disabled', 'acct.register-disabled')
admin_login()
global_settings('admin.settings-registration', uncheck=[('pnlOther', 'cbUsernamesEnabled')], select=('pnlOther', 'ddlRegistrationMethod', '1'))
sql('settings-after-modes', SETTINGS_CHANGED, 'admin.settings-registration,admin.settings-captcha,admin.settings-store-closed')

# ---------------------------------------------------------------------------------------------------------------------
phase('12. The administration: exports and files, the settings pages saved, the system pages\' actions; a downloadable product bought and downloaded')
admin_login()
for page, button, name, content in [
        ('products.aspx', 'ctrlProducts_btnExportXML', 'export-products-xml', 'xml'),
        ('products.aspx', 'ctrlProducts_btnPDFExport', 'export-products-pdf', 'pdf'),
        ('orders.aspx', 'ctrlOrders_btnExportXML', 'export-orders-xml', 'xml'),
        ('orders.aspx', 'ctrlOrders_btnPrintPdfPackagingSlips', 'export-orders-packaging-slips', 'pdf'),
        ('customers.aspx', 'ctrlCustomers_btnExportXML', 'export-customers-xml', 'xml'),
        ('categories.aspx', 'ctrlCategories_btnExportXML', 'export-categories-xml', 'xml'),
        ('manufacturers.aspx', 'ctrlManufacturers_btnExportXML', 'export-manufacturers-xml', 'xml'),
        ('LanguageDetails.aspx?LanguageId=7', 'btnXmlExport', 'export-language-xml', 'xml')]:
    goto(A + page)
    s('download', 'admin.' + name, target=button, name=name, content=content)
# Excel's exports: through OLE DB's Jet provider (Microsoft.Jet.OLEDB.4.0), 32-bit only: refused in a 64-bit process (the message shown).
for page, button, name in [('products.aspx', 'ctrlProducts_btnExportXLS', 'export-products-xls'), ('orders.aspx', 'ctrlOrders_btnExportXLS', 'export-orders-xls'),
                           ('customers.aspx', 'ctrlCustomers_btnExportXLS', 'export-customers-xls')]:
    goto(A + page)
    click(button, 'admin.' + name)
    snap(name)
goto(A + 'newslettersubscribers.aspx')
click('btnExportCVS', 'admin.export-newsletter-csv')
s('download', 'admin.export-newsletter-csv', target='btnExportCVSOk', name='export-newsletter-csv', content='text')
# Import e-mails from CSV: the Import button's confirmation (ConfirmButtonExtender with a ModalPopupExtender) shows the
# popup with the file input; its OK posts the form (the file) as the Import button's click. Lines "e-mail<TAB>active":
# a new subscriber, an existing one turned inactive, a line of another shape (left out).
goto(A + 'newslettersubscribers.aspx')
click('btnImportCSV', 'admin.import-newsletter-csv')
s('upload', target='fuCsvFile', value='subscribers.csv:import@example.com\tTrue\r\nnews@example.com\tfalse\r\nnot a line\r\n')
click('btnImportCSVOk', 'admin.import-newsletter-csv')
snap('newsletter-imported')
sql('newsletter-rows-imported', 'SELECT Email, Active FROM Nop_NewsLetterSubscription ORDER BY NewsLetterSubscriptionID', 'admin.import-newsletter-csv')
# The price list's handler (pricelist.csv?pricelist=...), the Froogle feed generated into the site's files.
goto(A + 'pricelist.aspx', 'pricelists-after', 'admin.pricelists')
fetch('/pricelist.csv?pricelist=parity', 'pricelist-csv-unknown', 'handler.pricelist')
goto(A + 'promotionproviders.aspx')
tab('pnlFroogle')
click('btnFroogleGenerate', 'admin.froogle-generate')
snap('froogle-generated')
# Every settings page saved as it is (its settings unchanged: the table compared after).
for page, button in [('blogsettings.aspx', 'ctrlBlogSettings_btnSave'), ('newssettings.aspx', 'ctrlNewsSettings_btnSave'),
                     ('shippingsettings.aspx', 'ctrlShippingSettings_btnSave'), ('taxsettings.aspx', 'ctrlTaxSettings_btnSave'),
                     ('paymentmethods.aspx', 'ctrlPaymentMethods_btnSave'), ('shippingmethods.aspx', 'ctrlShippingMethods_btnSave'),
                     ('taxproviders.aspx', 'ctrlTaxProviders_btnSave'), ('currencies.aspx', 'ctrlCurrencies_btnSave'),
                     ('measures.aspx', 'ctrlMeasures_btnSave'), ('emailaccounts.aspx', 'ctrlEmailAccounts_btnSave'),
                     ('activitytypes.aspx', 'ctrlActivityTypes_btnSave'), ('acl.aspx', 'ctrlACL_btnSave'),
                     ('smsproviders.aspx', 'ctrlSMSProviders_btnSave'), ('thirdpartyintegration.aspx', 'ctrlThirdPartyIntegration_btnSave'),
                     ('promotionproviders.aspx', 'ctrlPromotionProviders_btnSave'), ('onlinecustomers.aspx', 'ctrlOnlineCustomers_btnSave'),
                     ('forumssettings.aspx', 'ctrlForumsSettings_btnSave'), ('globalsettings.aspx', 'ctrlGlobalSettings_btnSave')]:
    goto(A + page)
    click(button, 'admin.settings-save-' + page[:-5])
    snap('settings-saved-' + page[:-5])
sql('settings-all', 'SELECT Name, Value FROM Nop_Setting ORDER BY Name', 'admin.settings-save-globalsettings')
# The SMTP test (the account's server is 127.0.0.1:9: refused).
goto(A + 'EmailAccountDetails.aspx?EmailAccountID=1')
fill('txtSendEmailTo_txtValue', 'test@example.com')
click('btnSendTestEmail', 'admin.email-account-test')
snap('email-account-test-sent')
# System: the cache cleared, the log searched and cleared, the activity log cleared, maintenance.
goto(A + 'default.aspx')
click('lbClearCache', 'admin.clear-cache')
snap('cache-cleared')
goto(A + 'logs.aspx', None, 'admin.logs')
# The e-mails still to be sent (the Emails task tries each one 3 times, a minute apart; the SMTP server is refused) taken as
# tried: their failures would log entries into the cleared log at a time of their own (found by recording twice on IIS).
sql(None, 'UPDATE Nop_QueuedEmail SET SendTries = 3 WHERE SendTries < 3')
click('ctrlLogs_btnClear', 'admin.logs-clear')
snap('logs-cleared')
goto(A + 'activitylog.aspx')
click('ClearButton', 'admin.activity-log-clear')
snap('activity-log-cleared')
goto(A + 'maintenance.aspx')
click('btnDeleteOldExportedFiles', 'admin.maintenance-delete-exported')
snap('maintenance-exported-deleted')
# A downloadable product (Poker Face) bought by the customer with a check, paid by the administrator, downloaded from the account.
s('newcontext')
login('parity@example.com', 'Parity-1')
goto('/products/59-poker-face.aspx')
click('btnAddToCart', 'cart.add-download')
s('check', target='cbTermsOfService')
click('btnCheckout')
s('click', target="css=[id$='dlShippingAddresses_ctl00_btnSelect']")
s('click', target="css=[id$='dlBillingAddresses_ctl00_btnSelect']")
s('check', target='dlShippingOptions_ctl00_rdShippingOption')
s('click', target="css=[id$='ctrlCheckoutShippingMethod_btnNextStep']")
s('check', target='dlPaymentMethod_ctl03_rdPaymentMethod')
s('click', target="css=[id$='ctrlCheckoutPaymentMethod_btnNextStep']")
script(None, NEXT_IF_SHOWN.replace('STEP', 'ctrlCheckoutPaymentInfo'))
s('click', 'checkout.download-order', target="css=[id$='ctrlCheckoutConfirm_btnNextStep']")
snap('checkout-download-completed')
goto('/account.aspx')
tab('pnlDP', 'acct.downloadable-products')
snap('account-downloads-unpaid')
admin_login()
goto(A + 'OrderDetails.aspx?OrderId=6')
confirm('btnMarkAsPaid', 'cbMarkAsPaid', 'admin.order-mark-paid')
snap('order-6-paid')
s('newcontext')
login('parity@example.com', 'Parity-1')
goto('/account.aspx')
tab('pnlDP', 'acct.downloadable-products')
snap('account-downloads-paid')
s('download', 'handler.getdownload', target="css=a[href*='getdownload.ashx']", name='purchased-download')
sql('order-lines-downloads', 'SELECT v.OrderID, v.ProductVariantID, v.DownloadCount, v.IsDownloadActivated FROM Nop_OrderProductVariant v WHERE v.OrderID = 6', 'handler.getdownload')
fetch('/getdownload.ashx?orderproductvariantguid=00000000-0000-0000-0000-000000000000', 'download-unknown', 'handler.getdownload')
fetch('/getlicense.ashx?orderproductvariantguid=00000000-0000-0000-0000-000000000000', 'license-unknown', 'handler.getlicense')
fetch('/QBConnector.asmx?wsdl', 'qbconnector-wsdl', 'handler.qbconnector', content='xml')
fetch('/Administration/GetDownloadAdmin.ashx?downloadid=1', 'admin-download-anonymous', 'handler.getdownload-admin')
# ---------------------------------------------------------------------------------------------------------------------
phase('13. Pages that only an outside party calls (payment gateways\' returns and notifications, without their data), the installer, the editor\'s file browser')
for page in GATEWAY_PAGES:
    fetch(f'/{page}.aspx', 'gateway-' + page.lower(), 'gateway.' + page.lower(), follow=True)
fetch('/checkout.aspx', 'checkout-page-redirect', 'checkout.page', follow=True)
fetch('/install/install.aspx', 'install-page-installed', 'install.page', follow=True)
fetch('/editors/fckeditor/editor/filemanager/connectors/aspx/connector.aspx?Command=GetFoldersAndFiles&Type=Image&CurrentFolder=/',
      'fck-connector-anonymous', 'admin.fck-file-browser', content='xml')
admin_login()
fetch('/editors/fckeditor/editor/filemanager/connectors/aspx/connector.aspx?Command=GetFoldersAndFiles&Type=Image&CurrentFolder=/',
      'fck-connector-admin', 'admin.fck-file-browser', content='xml')
fetch('/editors/fckeditor/editor/filemanager/connectors/aspx/upload.aspx?Type=Image', 'fck-upload', 'admin.fck-upload',
      upload='NewFile=parity.png:' + png(8, 8), bodyMatch='OnUploadCompleted\\([^)]*\\)')
fetch('/editors/fckeditor/editor/filemanager/connectors/aspx/connector.aspx?Command=GetFoldersAndFiles&Type=Image&CurrentFolder=/',
      'fck-connector-after-upload', 'admin.fck-upload', content='xml')
fetch('/images/image/parity.png', 'fck-uploaded-file', 'admin.fck-upload')

# The final state of the store: orders, customers.
sql('orders-final', ORDERS, 'state.final')
sql('customers-final', CUSTOMERS, 'state.final')

here = os.path.dirname(os.path.abspath(__file__))
scenario = {
    '_comment': 'nopCommerce 1.90: every user-facing operation of corpora/regression/nop.operations.json (nop.operations.md), each step\'s op naming the operations it exercises. Golden: corpora/parity/nop.operations.golden.json, recorded on the original on IIS by corpora/record-nop-operations.ps1; verified by experiments/wf4c/verify-windows.ps1 -Name nop -Suite operations and experiments/wf4c/run-linux.ps1 ... -Suite operations. Every run starts from the same database (corpora/nop-database.ps1: the installer\'s sample data; the SMTP server and the store URL 127.0.0.1:9); the sql steps read it (--sql). The administrator is the sample\'s admin@yourstore.com / admin. External requests of the browser are stubbed (stubExternal).',
    'operations': 'nop.operations.json',
    'allowed': '../parity/nop.operations.allowed.json',
    'textMasks': [
        '{today}',
        '(?<=T)\\d{2}:\\d{2}:\\d{2}(\\.\\d+)?',
        '\\b\\d{1,2}:\\d{2}(:\\d{2}(\\.\\d+)?)?( ?[AP]M)?\\b',
        '[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}',
        '(?<=© )20\\d\\d',
        # The PDF invoice's file name: the order's time (order_{date}_{HH-mm-ss}.pdf).
        '(?<=(order|packagingslip)_)[^ ]*(?=\\.pdf)',
        # An export's file name: the time after its date (products_{date}-HH-mm-ss[_random].xml).
        '(?<=\\{masked\\})-\\d{2}-\\d{2}-\\d{2}(_\\d+)?',
        # A gift card's coupon code: the start of a new GUID (GiftCardHelper.GenerateGiftCardCode).
        '\\b[0-9a-f]{8}-[0-9a-f]{4}\\b',
        # A date relative to the run (the administration's date pickers' defaults: today - 2 days, today + 1 year).
        '\\b\\d{1,2}/\\d{1,2}/20[2-9]\\d\\b',
        # The customers' export: a password's hash and its salt (a random salt for each customer the scenario registers).
        '(?<=<PasswordHash> )[0-9A-F]{40}\\b',
        '(?<=<SaltKey> )\\S+',
        # A log entry's id: after the e-mails' sending failures, as many as the task's timing made (log-entries compares the others).
        '(?<=LogID=)\\d+',
        # The folder the application was built in, in the stack traces of the log's exceptions (the PDBs' source paths: the
        # original's build, the conversion's).
        '[A-Za-z]:\\\\[^ :]*?\\\\(?=(NopCommerceStore|Libraries|Payment|Shipping|Tax|PromotionProviders)\\\\)',
    ],
    # Ajax Control Toolkit's tab headers: their text read differently (4.1's inline spans one line, 20.1's links a line each;
    # nop.operations.allowed.json).
    'ignoreSelectors': ['.ajax__tab_header'],
    'maskIds': ['[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}'],
    'waitMs': 300,
    'quietMs': 600,
    'continueOnError': True,
    'stubExternal': True,
    'captureUrl': True,
    'steps': STEPS,
}
out = sys.argv[1] if len(sys.argv) > 1 else os.path.join(here, 'nop.operations.scenario.json')
with open(out, 'w', encoding='utf-8', newline='\n') as f:
    json.dump(scenario, f, ensure_ascii=False, indent=1)
    f.write('\n')
# The inventory's operations and the steps' agree: every op of a step is one of it; its runnable ones are all exercised.
inventory = json.load(open(os.path.join(here, 'nop.operations.json'), encoding='utf-8'))['operations']
known = {o['id'] for o in inventory}
used = {o.strip() for st in STEPS for o in (st.get('op') or '').split(',') if o.strip()}
unknown = sorted(used - known)
unused = sorted(o['id'] for o in inventory if o['runnable'] and o['id'] not in used)
if unknown or unused:
    print('ops not in the inventory:', unknown)
    print('runnable ops no step does:', unused)
print(len(STEPS), 'steps', sum(1 for x in STEPS if x.get('action') in ('snapshot', 'fetch', 'files', 'cookies', 'assets', 'postraw', 'download') or (x.get('action') in ('script', 'sql') and x.get('name'))), 'snapshots')
