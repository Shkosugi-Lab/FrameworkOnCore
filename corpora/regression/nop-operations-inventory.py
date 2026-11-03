# Writes corpora/regression/nop.operations.json (the inventory ParityTest reads) and nop.operations.md (the same, to read)
# from the table below: every user-facing operation of nopCommerce 1.90, found by reading the original's source (the pages
# of the store and of the administration, their user controls - Modules, Administration/Modules, Templates -, the master
# pages, the handlers and the web service, web.config's modules and handlers, the URL rewriting rules, the business logic's
# services behind the buttons) and by crawling the running application (ParityTest discover as the anonymous visitor, the
# scenario's customer and the administrator: corpora/regression/nop.discovered.*.json, the links, forms, inputs, buttons,
# postback targets, validators). Edit the table and run: python corpora/regression/nop-operations-inventory.py
import json, os, sys
sys.path.insert(0, os.path.dirname(os.path.abspath(__file__)))
from nop_operations_common import ADMIN_PAGES, GATEWAY_PAGES  # noqa: E402

A, C, ADM, ANY = 'anonymous', 'customer', 'administrator', 'any'
OPS = []


def op(id, category, page, element, expected, role=ANY, pre=None, inputs=None, keys=None, runnable=True, reason=None, partial=None):
    OPS.append({k: v for k, v in dict(id=id, category=category, page=page, element=element, preconditions=pre, role=role,
                                       inputs=inputs, expected=expected, keys=keys, runnable=runnable, reason=reason,
                                       partial=partial).items() if v is not None})


def page_keys(path):
    """A page's discovery keys: its links, inputs, buttons, postback targets and validators (the path in lower case)."""
    p = path.lower()
    return [f'GET {p}*', f'INPUT {p}* *', f'SUBMIT {p}* *', f'POSTBACK {p}* *', f'VALIDATOR {p}* *', f'API * {p}*']


SMTP = 'the e-mail is queued (Nop_QueuedEmail, compared); its sending fails on both (the accounts\' SMTP server is 127.0.0.1:9, corpora/nop-database.ps1)'

# --- the store's pages (anonymous browsing: Default.aspx, Category.aspx, ... and their modules) ---
op('page.root', 'page', '/', 'the site\'s root: UrlRewritingNet\'s defaultPage redirect (the module sees "/" before the default document)', '302 to /default.aspx', keys=['GET /'])
op('page.home', 'page', '/default.aspx', 'HomePageCategories, HomePageProducts (featured), HomePageNews, BestSellers, HomePagePoll, the welcome topic', 'the home page', keys=page_keys('/default.aspx'))
op('page.home-categories', 'page', '/default.aspx', 'HomePageCategories (categories shown on the home page)', 'the categories\' boxes')
op('page.home-news', 'page', '/default.aspx', 'HomePageNews (Display.ShowNewsOnMainPage)', 'the latest news')
op('page.home-text', 'page', '/default.aspx', 'the HomePageText topic', 'the welcome text')
op('page.header-footer', 'page', 'every page', 'Header (login/register/cart/wishlist links, currency selector), HeaderMenu (search box), Footer (store theme selector), the side columns (category and manufacturer navigation, popular tags, information block, mini cart, newsletter box, poll, recently viewed)',
   'the same on every page', keys=['INPUT * *ctrlHeader*', 'INPUT * *ctrlSearchBox*', 'SUBMIT * *ctrlSearchBox*', 'POSTBACK * *ctrlCurrencySelector*', 'INPUT * *NewsLetterSubscriptionBox*', 'SUBMIT * *NewsLetterSubscriptionBox*',
                                    'VALIDATOR * *NewsLetterSubscriptionBox*', 'INPUT * *ctrlPoll*', 'SUBMIT * *ctrlPoll*', 'VALIDATOR * *ctrlPoll*', 'INPUT * *ctrlStoreTheme*', 'POSTBACK * *ctrlStoreTheme*', 'EXTERNAL *'])
op('page.category', 'page', '/category/{id}-{name}.aspx, /category.aspx?categoryid=', 'Category.aspx (UrlRewritingNet\'s CategoryDetailsRewrite), its template', 'the category\'s products', keys=page_keys('/category') + page_keys('/category.aspx'))
op('page.category-template-grid', 'page', '/category/29-books.aspx', 'Templates/Categories/ProductsInGrid.ascx', 'the products in a grid')
op('page.category-template-lines1', 'page', '/category/50-desktops.aspx', 'Templates/Categories/ProductsInLines1.ascx', 'the products in lines')
op('page.category-subcategories', 'page', '/category/30-computers.aspx', 'the sub-categories\' boxes', 'the sub-categories')
op('page.category-price-filter', 'page', '/category/49-jewelry.aspx?price=', 'PriceRangeFilter (the category\'s price ranges)', 'the products in the range')
op('page.category-spec-filter', 'page', '/category/51-notebooks.aspx?cputype=', 'ProductSpecificationFilter', 'the products with the option')
op('page.category-paging', 'page', '/category/...?pageindex=', 'the products\' pager', 'the second page')
op('page.category-unknown', 'page', '/category/9999-none.aspx', 'an unknown category', 'redirect to the home page')
op('page.manufacturers', 'page', '/manufacturers.aspx', 'the manufacturers list', 'the list', keys=page_keys('/manufacturers.aspx'))
op('page.manufacturer', 'page', '/manufacturer/{id}-{name}.aspx', 'Manufacturer.aspx (ProductsInGrid template)', 'the manufacturer\'s products', keys=page_keys('/manufacturer'))
op('page.product', 'page', '/products/{id}-{name}.aspx, /product.aspx?productid=', 'Product.aspx (ProductDetailsRewrite) and its template', 'the product', keys=page_keys('/products') + page_keys('/product.aspx'))
op('page.product-template-onevariant', 'page', '/products/45-...', 'Templates/Products/OneVariant.ascx: pictures (lightbox), price, quantity, add to cart / wishlist, tier prices, attributes', 'the product')
op('page.product-template-variantsingrid', 'page', '/products/67-...', 'Templates/Products/VariantsInGrid.ascx: each variant with its own buttons', 'the variants')
op('page.product-tabs', 'page', '/products/...', 'the reviews / specifications / tags tabs (Ajax Control Toolkit TabContainer)', 'the tabs')
op('page.product-attributes', 'page', '/products/67-..., /products/86-...', 'ProductAttributes: drop-downs, radio buttons, check boxes, text box', 'the attributes and their prices')
op('page.product-specifications', 'page', '/products/64-...', 'ProductSpecifications', 'the specification table')
op('page.product-related', 'page', '/products/...', 'RelatedProducts, ProductsAlsoPurchased', 'the related products')
op('page.product-gift-card', 'page', '/products/88-...', 'GiftCardAttributes (recipient, sender, message)', 'the gift card\'s form')
op('page.product-download-sample', 'page', '/products/58-...', 'the sample download link of a downloadable product', 'the link')
op('page.product-unknown', 'page', '/products/9999-none.aspx', 'an unknown product', 'redirect to the home page')
op('page.product-tag', 'page', '/producttag/{id}-{name}.aspx', 'ProductTag.aspx: the products with the tag, its pager', 'the products', keys=page_keys('/producttag'))
op('page.recently-added', 'page', '/recentlyaddedproducts.aspx', 'RecentlyAddedProducts', 'the newest products', keys=page_keys('/recentlyaddedproducts.aspx'))
op('page.recently-added-rss', 'page', '/recentlyaddedproductsrss.aspx', 'the RSS feed of the newest products', 'RSS 2.0', keys=['GET /recentlyaddedproductsrss.aspx*'])
op('page.recently-viewed', 'page', '/recentlyviewedproducts.aspx', 'RecentlyViewedProducts (the cookie NopCommerce.RecentlyViewedProducts)', 'the products viewed', keys=page_keys('/recentlyviewedproducts.aspx'))
op('page.sitemap', 'page', '/sitemap.aspx', 'the site map page', 'the categories, manufacturers, products, topics', keys=page_keys('/sitemap.aspx'))
op('page.sitemap-seo', 'page', '/sitemapseo.aspx', 'the sitemaps.org XML', 'the URLs', keys=['GET /sitemapseo.aspx*'])
op('page.topic', 'page', '/topic/{id}-{name}.aspx', 'Topic.aspx (TopicDetailsRewrite); an unknown topic', 'the topic; the home page', keys=page_keys('/topic'))
op('page.topic-pages', 'page', '/aboutus.aspx, /conditionsinfo.aspx, /conditionsinfopopup.aspx, /privacyinfo.aspx, /shippinginfo.aspx', 'the topics\' own pages', 'the topic',
   keys=page_keys('/aboutus.aspx') + page_keys('/conditionsinfo.aspx') + page_keys('/conditionsinfopopup.aspx') + page_keys('/privacyinfo.aspx') + page_keys('/shippinginfo.aspx'))
op('page.user-agreement', 'page', '/useragreement.aspx', 'the user agreement of a download (without one: the home page)', 'redirect to the home page', keys=page_keys('/useragreement.aspx'))
op('page.currency', 'page', 'every page', 'CurrencySelector (AutoPostBack): the working currency (the cookie Nop.CustomerCurrency)', 'the prices in euros; back in dollars')
op('page.contact', 'page', '/contactus.aspx', 'ContactUs: name, e-mail, enquiry', 'the form', keys=page_keys('/contactus.aspx'))
op('page.contact-validation', 'page', '/contactus.aspx', 'the form\'s validators', 'the messages')
op('page.contact-send', 'page', '/contactus.aspx', 'the enquiry sent', 'the confirmation; the e-mail to the store queued', partial=SMTP)
op('page.theme-selector', 'page', 'every page', 'StoreThemeSelector (Display.AllowCustomerSelectTheme): darkOrange, nopClassic', 'the page in the theme (its styles); the theme kept for the customer')
op('page.store-closed', 'page', 'every page', 'Common.StoreClosed: the store redirects to StoreClosed.htm; the administration stays', 'the closed page; the administration', keys=['GET /storeclosed.htm*'])

# --- search ---
op('search.page', 'search', '/search.aspx', 'Search: the term, advanced search (category, manufacturer, prices, descriptions)', 'the form', keys=page_keys('/search.aspx'))
op('search.header', 'search', 'every page', 'the header\'s search box', '/search.aspx?searchterms=')
op('search.query', 'search', '/search.aspx?searchterms=', 'the search from the URL', 'the results')
op('search.simple', 'search', '/search.aspx', 'a term', 'the products whose name has it')
op('search.descriptions', 'search', '/search.aspx', 'advanced: search in the descriptions too', 'more products')
op('search.advanced', 'search', '/search.aspx', 'advanced: a category, a manufacturer, a price range', 'the products in them')
op('search.paging', 'search', '/search.aspx', 'the results\' DataPager (NextPreviousPagerField: next, last, previous, first)', 'the pages of the results')
op('search.too-short', 'search', '/search.aspx', 'a term shorter than 3 characters', 'the message')
op('search.no-results', 'search', '/search.aspx', 'a term nothing has', 'the message')
op('search.log', 'search', '/search.aspx', 'the search log (Nop_SearchLog)', 'each term saved')

# --- product actions ---
op('product.email-friend-anonymous', 'product', '/productemailafriend.aspx', 'e-mail a friend, anonymous (not allowed by the sample\'s settings)', 'the message', A, keys=page_keys('/productemailafriend.aspx'))
op('product.email-friend-validation', 'product', '/productemailafriend.aspx', 'the friend\'s e-mail required', 'the message', C)
op('product.email-friend', 'product', '/productemailafriend.aspx', 'e-mail a friend', 'sent; the e-mail queued', C, partial=SMTP)
op('product.review-anonymous', 'product', '/productreviewadd.aspx', 'write a review, anonymous (not allowed)', 'the message', A, keys=page_keys('/productreviewadd.aspx'))
op('product.review-validation', 'product', '/productreviewadd.aspx', 'title and text required', 'the messages', C)
op('product.review-add', 'product', '/productreviewadd.aspx', 'a review with a rating', 'saved, waiting for approval (Nop_ProductReview)', C)
op('product.rating', 'product', '/products/{id}-...', 'ProductRating: the Ajax Control Toolkit Rating (AutoPostBack, its Changed event)', 'the vote saved (Nop_Product.RatingSum), "Rating will be updated"', C)

# --- cart, compare, wishlist ---
op('cart.view', 'cart', '/shoppingcart.aspx', 'ShoppingCart: OrderSummary, OrderTotals, discount and gift card boxes, estimate shipping, cross-sells', 'the cart', keys=page_keys('/shoppingcart.aspx'))
op('cart.compare', 'cart', '/compareproducts.aspx', 'CompareProducts (the cookie NopCommerce.CompareProducts)', 'the products side by side', keys=page_keys('/compareproducts.aspx'))
op('cart.compare-add', 'cart', '/products/...', 'ProductAddToCompareList', 'the product in the list')
op('cart.compare-clear', 'cart', '/compareproducts.aspx', 'Clear list', 'the list emptied')
op('cart.add-quantity-validation', 'cart', '/products/...', 'the quantity\'s validators', 'the message')
op('cart.add-one-variant', 'cart', '/products/...', 'Add to cart (one variant)', 'Nop_ShoppingCartItem')
op('cart.add-attributes-required', 'cart', '/products/67-...', 'required attributes not chosen', 'the warnings')
op('cart.add-attributes', 'cart', '/products/67-...', 'attributes chosen', 'the item with its AttributesXML and price')
op('cart.add-text-attribute', 'cart', '/products/86-...', 'a text attribute', 'the item with the text')
op('cart.add-variant-grid', 'cart', '/products/85-...', 'a variant of the grid', 'the variant in the cart')
op('cart.add-gift-card-validation', 'cart', '/products/88-...', 'the gift card\'s fields required', 'the messages')
op('cart.add-gift-card', 'cart', '/products/88-...', 'a gift card', 'the item with its recipient')
op('cart.add-from-list', 'cart', '/category/...', 'ProductBox1\'s Add to cart', 'the product in the cart')
op('cart.add-download', 'cart', '/products/59-...', 'a downloadable product', 'the product in the cart')
op('cart.update-quantity', 'cart', '/shoppingcart.aspx', 'Update shopping cart: a quantity', 'the quantity and totals')
op('cart.remove', 'cart', '/shoppingcart.aspx', 'Remove: an item checked', 'the item removed')
op('cart.discount-wrong', 'cart', '/shoppingcart.aspx', 'a coupon code no discount has', 'the message')
op('cart.discount-apply', 'cart', '/shoppingcart.aspx', 'the sample\'s coupon codes (their requirements unmet: not applied); the administrator\'s PARITY discount', 'the messages; the discount')
op('cart.gift-card-wrong', 'cart', '/shoppingcart.aspx', 'a gift card code that is none', 'the message')
op('cart.gift-card-apply', 'cart', '/shoppingcart.aspx', 'the gift card bought in the first order (activated by the administrator)', 'its amount in the totals', C)
op('cart.gift-card-remove', 'cart', '/shoppingcart.aspx', 'the gift card\'s remove link', 'removed from the totals', C)
op('cart.estimate-shipping', 'cart', '/shoppingcart.aspx', 'EstimateShipping: country (AutoPostBack: its states), zip; Get quote', 'the shipping options')
op('cart.continue-shopping', 'cart', '/shoppingcart.aspx', 'Continue shopping', 'back to the last category')
op('cart.carried-over', 'cart', '/shoppingcart.aspx', 'the anonymous visitor\'s cart after registering', 'the same items', C)
op('wishlist.view', 'wishlist', '/wishlist.aspx', 'Wishlist', 'the list', keys=page_keys('/wishlist.aspx'))
op('wishlist.add', 'wishlist', '/products/...', 'Add to wishlist', 'Nop_ShoppingCartItem type 2')
op('wishlist.update', 'wishlist', '/wishlist.aspx', 'a quantity updated', 'the quantity')
op('wishlist.to-cart', 'wishlist', '/wishlist.aspx', 'Add to cart from the wishlist', 'the item in the cart')
op('wishlist.remove', 'wishlist', '/wishlist.aspx', 'items removed', 'the list emptied')
op('wishlist.email-anonymous', 'wishlist', '/wishlistemailafriend.aspx', 'e-mail the wishlist, anonymous', 'not allowed', A, keys=page_keys('/wishlistemailafriend.aspx'))
op('wishlist.email', 'wishlist', '/wishlistemailafriend.aspx', 'e-mail the wishlist to a friend', 'sent; queued', C, partial=SMTP)
op('wishlist.shared', 'wishlist', '/wishlist.aspx?customerguid=', 'another customer\'s wishlist by its GUID (the link of the e-mail: the GUID made a known one); an unknown GUID', 'the customer\'s list; the visitor\'s own')

# --- newsletter, poll ---
op('newsletter.validation', 'newsletter', 'every page', 'the newsletter box\'s e-mail required', 'the message')
op('newsletter.subscribe', 'newsletter', 'every page', 'subscribe', 'an inactive subscription; the activation e-mail queued', partial=SMTP)
op('newsletter.activate', 'newsletter', '/newslettersubscriptionactivation.aspx?t=&active=', 'the activation / deactivation link (its GUID made a known one); an unknown GUID', 'activated, deactivated; the message', keys=page_keys('/newslettersubscriptionactivation.aspx'))
op('newsletter.unsubscribe', 'newsletter', 'every page', 'unsubscribe', 'the subscription inactive')
op('poll.validation', 'poll', 'every page (the side column)', 'Vote without an answer', 'the message')
op('poll.vote', 'poll', 'every page (the side column)', 'a vote', 'the results; Nop_PollAnswer.Count')

# --- account ---
op('acct.login-page', 'account', '/login.aspx', 'CustomerLogin: asp:Login, register / checkout as guest', 'the form', keys=page_keys('/login.aspx'))
op('acct.login-validation', 'account', '/login.aspx', 'the Login control\'s validators', 'the marks', A)
op('acct.login-wrong', 'account', '/login.aspx', 'a wrong password (StoreMembershipProvider)', 'the failure text', A)
op('acct.login-unknown', 'account', '/login.aspx', 'an unknown e-mail', 'the failure text', A)
op('acct.login', 'account', '/login.aspx', 'login, remember me', 'the persistent NOPCOMMERCE.AUTH cookie', A)
op('acct.logout', 'account', '/logout.aspx', 'Log out', 'anonymous again', C, keys=page_keys('/logout.aspx'))
op('acct.register-page', 'account', '/register.aspx', 'CustomerRegister: a CreateUserWizard', 'the form', keys=page_keys('/register.aspx'))
op('acct.register-validation', 'account', '/register.aspx', 'required fields, an invalid e-mail, passwords that differ', 'the messages', A)
op('acct.register', 'account', '/register.aspx', 'a customer registered (with the newsletter)', 'Nop_Customer, its attributes and addresses; logged in; the welcome e-mail queued', A, partial=SMTP)
op('acct.register-continue', 'account', '/register.aspx', 'Continue after the registration', 'the home page', C)
op('acct.register-duplicate', 'account', '/register.aspx', 'an e-mail already registered', 'the message', A)
op('acct.register-email-validation', 'account', '/register.aspx', 'Common.CustomerRegistrationType email validation, usernames enabled', 'registered inactive; the activation e-mail queued', A, partial=SMTP)
op('acct.register-disabled', 'account', '/register.aspx', 'registration disabled', 'the topic RegistrationNotAllowed', A)
op('acct.activation', 'account', '/accountactivation.aspx?act=&email=', 'the activation link (its token made a known one); a wrong token', 'activated; the home page', keys=page_keys('/accountactivation.aspx'))
op('acct.captcha', 'account', '/login.aspx, /register.aspx', 'the captcha image (Common.LoginCaptchaImageEnabled, RegisterCaptchaImageEnabled)', 'the image and its field')
op('acct.captcha-wrong', 'account', '/login.aspx', 'a wrong captcha answer', 'the message', partial='the image\'s text is random (CaptchaImage.aspx: kept in the session): only a wrong answer can be compared')
op('acct.password-recovery-page', 'account', '/passwordrecovery.aspx', 'CustomerPasswordRecovery', 'the form', keys=page_keys('/passwordrecovery.aspx'))
op('acct.password-recovery-unknown', 'account', '/passwordrecovery.aspx', 'an unknown e-mail', 'the message')
op('acct.password-recovery-send', 'account', '/passwordrecovery.aspx', 'a customer\'s e-mail', 'the token saved; the e-mail queued', partial=SMTP)
op('acct.password-recovery-token', 'account', '/passwordrecovery.aspx?prt=&email=', 'the e-mail\'s link (its token made a known one); a wrong token', 'the new password form; the home page')
op('acct.password-recovery-new', 'account', '/passwordrecovery.aspx?prt=', 'the new password (mismatch, then set); login with it', 'the password changed')
op('acct.account-anonymous', 'account', '/account.aspx', 'My account without a login', 'redirect to the login', A, keys=page_keys('/account.aspx'))
op('acct.account', 'account', '/account.aspx', 'the account\'s tabs (info, addresses, orders, returns, downloads, reward points, password, avatar, forum subscriptions)', 'the tabs', C)
op('acct.account-tabs', 'account', '/account.aspx', 'the tabs switched', 'each tab', C)
op('acct.info-save', 'account', '/account.aspx', 'CustomerInfo saved', 'the attributes (FirstName, City...)', C)
op('acct.address-add', 'account', '/addressedit.aspx', 'Add a billing / shipping address', 'Nop_Address', C, keys=page_keys('/addressedit.aspx'))
op('acct.address-validation', 'account', '/addressedit.aspx', 'the address\'s required fields', 'the messages', C)
op('acct.address-edit', 'account', '/addressedit.aspx?addressid=', 'an address edited', 'the address', C)
op('acct.address-delete', 'account', '/addressedit.aspx?addressid=', 'an address deleted', 'removed', C)
op('acct.address-other', 'account', '/addressedit.aspx?addressid=77', 'another customer\'s address', 'not shown (redirect)', C)
op('acct.change-password-wrong', 'account', '/account.aspx', 'a wrong current password', 'the message', C)
op('acct.change-password-validation', 'account', '/account.aspx', 'the new passwords differ', 'the message', C)
op('acct.change-password', 'account', '/account.aspx', 'the password changed', 'changed', C)
op('acct.orders', 'account', '/account.aspx', 'CustomerOrders', 'the orders', C)
op('acct.order-details', 'account', '/orderdetails.aspx?orderid=', 'OrderDetails', 'the order', C, keys=page_keys('/orderdetails.aspx'))
op('acct.order-other', 'account', '/orderdetails.aspx?orderid=1', 'an order that is not the customer\'s (before it exists)', 'redirect', C)
op('acct.order-pdf', 'account', '/orderdetails.aspx', 'PDF invoice (PdfHelper, MigraDoc)', 'a PDF of a page', C)
op('acct.order-print', 'account', '/printorderdetails.aspx?orderid=', 'the printable order', 'the order', C, keys=page_keys('/printorderdetails.aspx'))
op('acct.reorder', 'account', '/orderdetails.aspx', 'Re-order', 'the order\'s items in the cart', C)
op('acct.return-items-page', 'account', '/returnitems.aspx?orderid=', 'ReturnItems (only for a complete order)', 'the form; the home page before', C, keys=page_keys('/returnitems.aspx'))
op('acct.return-items', 'account', '/returnitems.aspx', 'a return request', 'Nop_ReturnRequest', C)
op('acct.return-requests', 'account', '/account.aspx', 'CustomerReturnRequests', 'the request', C)
op('acct.reward-points', 'account', '/account.aspx', 'CustomerRewardPoints', 'the balance and history', C)
op('acct.downloadable-products', 'account', '/account.aspx', 'CustomerDownloadableProducts', 'the downloads (n/a until paid)', C)
op('acct.forum-subscriptions', 'account', '/account.aspx', 'ForumSubscriptions', 'the topic watched', C)
op('acct.avatar', 'account', '/account.aspx', 'CustomerAvatar (Customer.CustomersAllowedToUploadAvatars)', 'the form', C)
op('acct.avatar-upload', 'account', '/account.aspx', 'an avatar uploaded (a JPEG; resized)', 'the avatar', C)
op('acct.avatar-remove', 'account', '/account.aspx', 'the avatar removed', 'no avatar', C)
op('acct.profile', 'account', '/profile.aspx?UserId=', 'ProfileInfo (Customer.AllowViewingProfiles): statistics, latest posts, PM button', 'the profile', keys=page_keys('/profile.aspx'))

# --- checkout ---
op('checkout.page', 'checkout', '/checkout.aspx', 'the checkout\'s entry', 'redirect to the cart / the first step', keys=page_keys('/checkout.aspx'))
op('checkout.anonymous-to-login', 'checkout', '/shoppingcart.aspx', 'Checkout without a login (anonymous checkout not allowed)', 'the login page', A)
op('checkout.start', 'checkout', '/shoppingcart.aspx', 'Checkout', 'the shipping address step', C)
op('checkout.shipping-address', 'checkout', '/checkoutshippingaddress.aspx', 'CheckoutShippingAddress', 'the addresses and the form', C, keys=page_keys('/checkoutshippingaddress.aspx'))
op('checkout.shipping-address-validation', 'checkout', '/checkoutshippingaddress.aspx', 'a new address\'s required fields', 'the messages', C)
op('checkout.shipping-address-select', 'checkout', '/checkoutshippingaddress.aspx', 'Ship to this address', 'the billing step', C)
op('checkout.billing-address-select', 'checkout', '/checkoutbillingaddress.aspx', 'Billing to this address', 'the shipping method step', C, keys=page_keys('/checkoutbillingaddress.aspx'))
op('checkout.shipping-method', 'checkout', '/checkoutshippingmethod.aspx', 'a shipping option (Shipping By Order Weight: In-Store Pickup, By Ground, By Air)', 'the payment method step', C, keys=page_keys('/checkoutshippingmethod.aspx'))
op('checkout.payment-manual', 'checkout', '/checkoutpaymentmethod.aspx', 'Credit Card (Manual Processing)', 'the payment info step', C, keys=page_keys('/checkoutpaymentmethod.aspx'))
op('checkout.payment-info', 'checkout', '/checkoutpaymentinfo.aspx', 'the card (type, name, number, expiry, CVV2)', 'the confirm step', C, keys=page_keys('/checkoutpaymentinfo.aspx'))
op('checkout.confirm', 'checkout', '/checkoutconfirm.aspx', 'Confirm', 'the order (Nop_Order, its lines, notes; the card encrypted); the e-mails queued; the cart emptied', C, keys=page_keys('/checkoutconfirm.aspx'), partial=SMTP)
op('checkout.completed-details', 'checkout', '/checkoutcompleted.aspx', 'the order\'s details link', 'the order', C, keys=page_keys('/checkoutcompleted.aspx'))
op('checkout.terms-required', 'checkout', '/shoppingcart.aspx', 'Checkout.TermsOfServiceEnabled: checkout without accepting', 'the warning')
op('checkout.anonymous', 'checkout', '/shoppingcart.aspx', 'Checkout.AnonymousCheckoutAllowed: checkout as a visitor', 'login or checkout as a guest', A)
op('checkout.guest', 'checkout', '/login.aspx', 'Checkout as guest', 'the one-page checkout', A)
op('checkout.onepage', 'checkout', '/checkoutonepage.aspx', 'Checkout.UseOnePageCheckout: CheckoutOnePage (UpdatePanels: the steps in one page)', 'the steps', keys=page_keys('/checkoutonepage.aspx'))
op('checkout.onepage-shipping-address', 'checkout', '/checkoutonepage.aspx', 'a new shipping address; an address chosen', 'the billing step')
op('checkout.onepage-billing-address', 'checkout', '/checkoutonepage.aspx', 'the same as the shipping address; an address chosen', 'the shipping method step')
op('checkout.onepage-shipping-method', 'checkout', '/checkoutonepage.aspx', 'a shipping option', 'the payment method step')
op('checkout.onepage-payment-info', 'checkout', '/checkoutonepage.aspx', 'the payment info (none when the reward points pay)', 'the confirm step')
op('checkout.onepage-confirm', 'checkout', '/checkoutonepage.aspx', 'Confirm', 'the order')
op('checkout.payment-purchase-order', 'checkout', '/checkoutonepage.aspx', 'Purchase Order (its number)', 'the order with the PO number', A)
op('checkout.payment-cod', 'checkout', '/checkoutonepage.aspx', 'Cash On Delivery', 'the order', C)
op('checkout.payment-check', 'checkout', '/checkoutonepage.aspx', 'Check / Money Order (its instructions)', 'the order', C)
op('checkout.payment-in-store', 'checkout', '/checkoutonepage.aspx', 'Pay In Store', 'the order', C)
op('checkout.payment-card-validation', 'checkout', '/checkoutonepage.aspx', 'the card\'s validators (required, an invalid number)', 'the messages', C)
op('checkout.reward-points', 'checkout', '/checkoutonepage.aspx', 'use the reward points (RewardPoints.Enabled)', 'the order paid by points; the history', C)
op('checkout.download-order', 'checkout', '/checkoutonepage.aspx', 'an order of a downloadable product (no shipping)', 'the order', C)
op('checkout.paypal-express', 'checkout', '/shoppingcart.aspx, /checkoutpaymentmethod.aspx', 'PayPal Express\'s button', 'the PayPal site', runnable=False,
   reason='PayPal Express is inactive in the sample (and needs a PayPal account and the PayPal site); the active offline methods are run instead')
op('checkout.external-gateways', 'checkout', '/checkoutpaymentinfo.aspx', 'the 30 hosted / API gateways (Authorize.NET, PayPal Direct/Standard, 2Checkout, Google Checkout...)', 'their sites', runnable=False,
   reason='they need the gateways\' accounts and sites on the Internet; inactive in the sample. Their return and notification pages are asked without the gateways\' data (gateway.*)')

# --- forums, private messages ---
op('forum.groups', 'forum', '/Boards/default.aspx', 'Forums.ForumsEnabled: the forum groups', 'the groups', keys=page_keys('/boards/default.aspx') + page_keys('/boards'))
op('forum.group', 'forum', '/boards/fg/{id}/{name}.aspx', 'ForumGroup (ForumGroupRewrite)', 'the forums', keys=page_keys('/boards/fg') + page_keys('/boards/forumgroup.aspx'))
op('forum.forum', 'forum', '/boards/f/{id}/{name}.aspx', 'Forum (ForumRewrite): topics, New Topic, Watch Forum', 'the topics', keys=page_keys('/boards/f') + page_keys('/boards/forum.aspx'))
op('forum.topic-new-validation', 'forum', '/Boards/TopicNew.aspx', 'title and text required', 'the messages', C, keys=page_keys('/boards/topicnew.aspx'))
op('forum.topic-new', 'forum', '/Boards/TopicNew.aspx', 'a topic (BBCode)', 'Nop_Forums_Topic, Nop_Forums_Post', C)
op('forum.topic-open', 'forum', '/boards/t/{id}/{name}.aspx', 'a topic (ForumTopicRewrite)', 'its posts', keys=page_keys('/boards/t') + page_keys('/boards/topic.aspx'))
op('forum.post-reply', 'forum', '/Boards/PostNew.aspx', 'a reply', 'the post', C, keys=page_keys('/boards/postnew.aspx'))
op('forum.post-quote', 'forum', '/Boards/PostNew.aspx?QuotePostId=', 'a quote', 'the quoted post', C)
op('forum.post-edit', 'forum', '/Boards/TopicEdit.aspx, /Boards/PostEdit.aspx', 'a post edited (Forums.CustomersAllowedToEditPosts)', 'the text', C, keys=page_keys('/boards/topicedit.aspx') + page_keys('/boards/postedit.aspx'))
op('forum.watch-topic', 'forum', '/boards/t/...', 'Watch Topic', 'a subscription', C)
op('forum.watch-forum', 'forum', '/boards/f/...', 'Watch Forum', 'a subscription', C)
op('forum.search', 'forum', '/Boards/Search.aspx?searchterms=', 'ForumSearch', 'the topics', keys=page_keys('/boards/search.aspx'))
op('forum.search-advanced', 'forum', '/Boards/Search.aspx', 'advanced search (forum, within, period)', 'the posts')
op('forum.active-discussions', 'forum', '/Boards/ActiveDiscussions.aspx', 'ForumActiveDiscussions', 'the latest topics', keys=page_keys('/boards/activediscussions.aspx'))
op('forum.topic-move', 'forum', '/Boards/MoveTopic.aspx', 'Move Topic (administrator / moderator)', 'the topic in the other forum', ADM, keys=page_keys('/boards/movetopic.aspx'))
op('forum.topic-delete', 'forum', '/boards/t/...', 'Delete Topic', 'removed with its posts', ADM)
op('pm.send-page', 'pm', '/sendpm.aspx?toUserId=', 'PrivateMessagesSend (Customer.AllowPM)', 'the form', C, keys=page_keys('/sendpm.aspx'))
op('pm.send-validation', 'pm', '/sendpm.aspx', 'the subject required', 'the message', C)
op('pm.send', 'pm', '/sendpm.aspx', 'a private message', 'Nop_Forums_PrivateMessage; the inbox link (1 unread)', C)
op('pm.inbox', 'pm', '/privatemessages.aspx', 'PrivateMessagesInbox', 'the messages', keys=page_keys('/privatemessages.aspx'))
op('pm.sent-items', 'pm', '/privatemessages.aspx', 'PrivateMessagesSentItems', 'the messages sent', C)
op('pm.view', 'pm', '/viewpm.aspx?pm=', 'PrivateMessagesView', 'the message (read)', ADM, keys=page_keys('/viewpm.aspx'))
op('pm.reply', 'pm', '/viewpm.aspx', 'Reply', 'a message back', ADM)
op('pm.mark-unread', 'pm', '/privatemessages.aspx', 'Mark as unread', 'IsRead false', ADM)
op('pm.delete', 'pm', '/privatemessages.aspx', 'Delete selected', 'IsDeletedByRecipient', ADM)

# --- blog, news ---
op('blog.list', 'blog', '/blog.aspx', 'Blog: the posts, archive by month, popular tags', 'the posts', keys=page_keys('/blog.aspx'))
op('blog.tag', 'blog', '/blog.aspx?tag=', 'the posts with a tag', 'the posts')
op('blog.month', 'blog', '/blog.aspx?month=', 'the posts of a month', 'the posts')
op('blog.post', 'blog', '/blog/{id}-{name}.aspx', 'BlogPost (BlogDetailsRewrite), its comments', 'the post', keys=page_keys('/blog') + page_keys('/blogpost.aspx'))
op('blog.rss', 'blog', '/blogrss.aspx?languageid=', 'the blog\'s RSS', 'RSS 2.0', keys=['GET /blogrss.aspx*'])
op('blog.comment-validation', 'blog', '/blog/...', 'the comment required', 'the message')
op('blog.comment', 'blog', '/blog/...', 'a comment (Blog.AllowNotRegisteredUsersToLeaveComments)', 'Nop_BlogComment (the IP kept)')
op('news.archive', 'news', '/newsarchive.aspx', 'NewsArchive', 'the news', keys=page_keys('/newsarchive.aspx'))
op('news.item', 'news', '/news/{id}-{name}.aspx', 'NewsItem (NewsDetailsRewrite), its comments', 'the news', keys=page_keys('/news') + page_keys('/news.aspx'))
op('news.rss', 'news', '/newsrss.aspx?languageid=', 'the news\' RSS', 'RSS 2.0', keys=['GET /newsrss.aspx*'])
op('news.comment-validation', 'news', '/news/...', 'the comment required', 'the message')
op('news.comment', 'news', '/news/...', 'a comment with a title', 'Nop_NewsComment')

# --- handlers, static files, errors, state ---
op('handler.getdownload-sample', 'handler', '/getdownload.ashx?sampledownloadproductvariantid=', 'a product\'s sample download; a variant without one', 'the file (attachment); the home page', keys=['GET /getdownload.ashx*'])
op('handler.getdownload', 'handler', '/getdownload.ashx?orderproductvariantguid=', 'a bought download (after the order is paid); an unknown GUID', 'the file; the home page', C)
op('handler.getlicense', 'handler', '/getlicense.ashx?orderproductvariantguid=', 'a license file of an order\'s line; an unknown GUID', 'the home page', keys=['GET /getlicense.ashx*'],
   partial='an unknown GUID only (a license is uploaded by the administrator for an order\'s line: not done)')
op('handler.getdownload-admin', 'handler', '/Administration/GetDownloadAdmin.ashx?downloadid=', 'a download for the administration; without a login', 'redirect to the login', keys=['GET /administration/getdownloadadmin.ashx*'])
op('handler.froogle', 'handler', '/froogle.ashx', 'the Froogle (Google Base) feed', 'RSS with g: elements', keys=['GET /froogle.ashx*'])
op('handler.keepalive', 'handler', '/keepalive/ping.ashx', 'KeepAlive (the KeepAliveTask asks it)', 'an empty answer', keys=['GET /keepalive/ping.ashx*'])
op('handler.captcha-image', 'handler', '/captchaimage.aspx', 'the captcha\'s image (random text)', 'a JPEG (its bytes not compared: random)', keys=['GET /captchaimage.aspx*'])
op('handler.pricelist', 'handler', '/pricelist.csv?pricelist=', 'PricelistHandler (web.config httpHandlers): an unknown price list', 'the answer', keys=['GET /pricelist.csv*'])
op('handler.qbconnector', 'handler', '/QBConnector.asmx?wsdl', 'the QuickBooks Web Connector\'s ASMX service: its WSDL', 'the WSDL', keys=['GET /qbconnector.asmx*'],
   partial='the WSDL only: its operations are QuickBooks Web Connector\'s (QB.Enabled false in the sample; a QuickBooks installation calls them)')
op('static.files', 'static', '/robots.txt, /errorpage.htm, /filenotfound.htm', 'static files', 'the files', keys=['GET /robots.txt', 'GET /errorpage.htm*', 'GET /filenotfound.htm*', 'GET /bannedaddress.htm*'])
op('static.page-assets', 'static', 'every page', 'the scripts, styles, images (WebResource.axd, ScriptResource.axd, App_Themes), the products\' thumbnails made by the application (images/thumbs: System.Drawing)', 'each one\'s answer and bytes',
   keys=['GET /images/*', 'GET /app_themes/*', 'GET /scripts/*'])
op('error.not-found', 'error', 'a missing page or file', 'a URL nothing answers', '404 (IIS\'s for a file; customErrors Off)')
op('error.folders', 'error', '/administration, /Boards, /images/, /foo, /?a=1', 'a folder\'s URL or an extensionless one: the managed pipeline runs for it as IIS runs it (UrlRewritingNet\'s defaultPage redirect)', '302 to .../default.aspx (the query kept)')
op('state.cookies-anonymous', 'state', 'every page', 'the cookies of a visit', '.ASPXANONYMOUS (anonymousIdentification)')
op('state.cookies-authenticated', 'state', 'after login', 'the authentication cookie', 'NOPCOMMERCE.AUTH: session, or persistent with remember me; Nop.CustomerSessionGUIDCookie')
op('state.final', 'state', 'the database', 'the orders and customers at the end', 'the same rows')
op('install.page', 'install', '/install/install.aspx', 'the installer once the store is installed (the connection string set)', 'redirect to the store', keys=page_keys('/install/install.aspx'))
op('install.run', 'install', '/install/install.aspx', 'the installer\'s wizard: the database created, the scripts run, the connection string saved', 'a new store', runnable=False,
   reason='every run starts from the installed sample store (corpora/nop-database.ps1 runs the installer\'s own scripts, as the installer does): running the installer would replace the database and ConnectionStrings.config under the running suite')
for page in GATEWAY_PAGES:
    op('gateway.' + page.lower(), 'gateway', f'/{page}.aspx', 'a payment gateway\'s return or notification page, asked without the gateway\'s data', 'its answer (the redirects followed)',
       keys=page_keys(f'/{page}.aspx'), partial='without the gateway\'s data: the gateways are outside services (their accounts and requests), inactive in the sample')

# --- administration: its login, every page ---
op('admin.login-page', 'admin', '/administration/login.aspx', 'the administration\'s Login', 'the form', A, keys=page_keys('/administration/login.aspx'))
op('admin.login-required', 'admin', '/administration/default.aspx', 'the administration without a login', 'redirect to its login', A)
op('admin.login-validation', 'admin', '/administration/login.aspx', 'the validators', 'the marks', A)
op('admin.login-wrong', 'admin', '/administration/login.aspx', 'a wrong password', 'the failure text', A)
op('admin.login', 'admin', '/administration/login.aspx', 'the administrator\'s login', 'the dashboard; NOPCOMMERCE.AUTH', A)
op('admin.access-denied', 'admin', '/administration/default.aspx', 'a customer in the administration', 'the login / access denied', C, keys=page_keys('/administration/accessdenied.aspx'))
op('admin.dashboard', 'admin', '/administration/default.aspx', 'the dashboard after the orders (statistics, the incomplete orders, the latest orders)', 'the numbers', ADM, keys=page_keys('/administration/default.aspx') + page_keys('/administration'))
for path, name in ADMIN_PAGES:
    p = path.split('?')[0]
    op('admin.page.' + name, 'admin-page', path, 'the page opened (its tabs, grids, forms as they are)', 'the page', ADM, keys=page_keys(p) + ['INPUT * *ctl00$ctrlMenu*', 'POSTBACK * ctl00$lbClearCache'])

# --- administration: the entities added, edited, deleted (Add page: Save and stay; Details: Save and stay, Save; Delete: Yes) ---
CRUD = [('manufacturer', 'manufacturer', 'Nop_Manufacturer (soft delete)'), ('category', 'category', 'Nop_Category (soft delete)'), ('product-attribute', 'product attribute', 'Nop_ProductAttribute'),
        ('specification-attribute', 'specification attribute', 'Nop_SpecificationAttribute'), ('checkout-attribute', 'checkout attribute', 'Nop_CheckoutAttribute'),
        ('product-template', 'product template', 'Nop_ProductTemplate'), ('category-template', 'category template', 'Nop_CategoryTemplate'),
        ('manufacturer-template', 'manufacturer template', 'Nop_ManufacturerTemplate'), ('customer-role', 'customer role', 'Nop_CustomerRole (soft delete)'),
        ('affiliate', 'affiliate', 'Nop_Affiliate (soft delete)'), ('campaign', 'campaign', 'Nop_Campaign'), ('discount', 'discount (kept: used in the cart)', 'Nop_Discount'),
        ('poll', 'poll', 'Nop_Poll'), ('news', 'news', 'Nop_News'), ('blog-post', 'blog post', 'Nop_BlogPost'), ('topic', 'topic', 'Nop_Topic'),
        ('forum-group', 'forum group', 'Nop_Forums_Group'), ('forum', 'forum', 'Nop_Forums_Forum'), ('email-account', 'e-mail account', 'Nop_EmailAccount'),
        ('blacklist-ip', 'banned IP address', 'Nop_BannedIpAddress'), ('blacklist-network', 'banned network', 'Nop_BannedIpNetwork'),
        ('credit-card-type', 'credit card type', 'Nop_CreditCardType'), ('tax-category', 'tax category', 'Nop_TaxCategory'), ('shipping-method', 'shipping method', 'Nop_ShippingMethod'),
        ('country', 'country', 'Nop_Country'), ('state-province', 'state / province', 'Nop_StateProvince'), ('currency', 'currency', 'Nop_Currency'),
        ('language', 'language (the culture list: CultureInfo.GetCultures)', 'Nop_Language'), ('warehouse', 'warehouse', 'Nop_Warehouse (soft delete)'),
        ('measure-dimension', 'dimension', 'Nop_MeasureDimension'), ('measure-weight', 'weight', 'Nop_MeasureWeight'), ('setting', 'setting', 'Nop_Setting'),
        ('pricelist', 'price list', 'Nop_Pricelist'), ('payment-method', 'payment method', 'Nop_PaymentMethod'),
        ('shipping-rate-method', 'shipping rate computation method', 'Nop_ShippingRateComputationMethod'), ('tax-provider', 'tax provider', 'Nop_TaxProvider'),
        ('customer', 'customer', 'Nop_Customer (soft delete)')]
for key, label, table in CRUD:
    op(f'admin.{key}-validation', 'admin-crud', f'the {label}\'s Add page', 'Save with nothing filled', 'the validators\' messages', ADM)
    op(f'admin.{key}-add', 'admin-crud', f'the {label}\'s Add page', f'a {label} added (Save and Continue Edit)', f'its details page; {table}', ADM)
    op(f'admin.{key}-edit', 'admin-crud', f'the {label}\'s Details page', 'edited (Save and Continue Edit; Save)', f'the list; {table}', ADM)
    if key != 'discount':
        op(f'admin.{key}-delete', 'admin-crud', f'the {label}\'s Details page', 'Delete (the confirmation\'s Yes)', f'the list; {table}', ADM)

# --- administration: catalog ---
CAT = 'admin-catalog'
op('admin.product-validation', CAT, '/administration/ProductAdd.aspx', 'Save with nothing filled', 'the messages', ADM)
op('admin.product-add', CAT, '/administration/ProductAdd.aspx', 'a product with its variant, tags, a category and a manufacturer', 'Nop_Product, Nop_ProductVariant, the mappings', ADM)
op('admin.product-edit', CAT, '/administration/ProductDetails.aspx', 'the product\'s name saved', 'saved', ADM)
op('admin.product-tabs', CAT, '/administration/ProductDetails.aspx', 'the product\'s tabs', 'each tab', ADM)
op('admin.product-picture-upload', CAT, '/administration/ProductDetails.aspx', 'a picture uploaded (a PNG; Media.Images.StoreInDB: Nop_Picture)', 'the picture listed', ADM)
op('admin.product-specification-add', CAT, '/administration/ProductDetails.aspx', 'a specification attribute option added', 'the mapping', ADM)
op('admin.product-storefront', CAT, '/products/96-...', 'the new product on the store: its page, its picture resized (images/thumbs: System.Drawing)', 'the page; the thumbnail\'s bytes', A)
op('admin.product-related-search', CAT, '/administration/RelatedProductAdd.aspx?pid=', 'the related products\' search popup', 'the products found', ADM)
op('admin.product-related-add', CAT, '/administration/RelatedProductAdd.aspx', 'a related product added', 'Nop_RelatedProduct', ADM)
op('admin.product-cross-sell-search', CAT, '/administration/CrossSellProductAdd.aspx?pid=', 'the cross-sells\' search popup', 'the products found', ADM)
op('admin.product-cross-sell-add', CAT, '/administration/CrossSellProductAdd.aspx', 'a cross-sell added', 'Nop_CrossSellProduct', ADM)
op('admin.variant-details', CAT, '/administration/ProductVariantDetails.aspx', 'a variant\'s details', 'the page', ADM)
op('admin.variant-edit', CAT, '/administration/ProductVariantDetails.aspx', 'the SKU saved', 'saved', ADM)
op('admin.variant-tier-price', CAT, '/administration/ProductVariantDetails.aspx', 'a tier price added', 'Nop_TierPrice', ADM)
op('admin.variant-role-price', CAT, '/administration/ProductVariantDetails.aspx', 'a price for a customer role', 'Nop_CustomerRole_ProductPrice', ADM)
op('admin.variant-attribute-add', CAT, '/administration/ProductVariantDetails.aspx', 'an attribute added to the variant', 'the mapping', ADM)
op('admin.variant-attribute-values', CAT, '/administration/ProductVariantAttributeValues.aspx', 'an attribute\'s values', 'the grid', ADM)
op('admin.variant-attribute-value-add', CAT, '/administration/ProductVariantAttributeValues.aspx', 'a value added', 'Nop_ProductVariantAttributeValue', ADM)
op('admin.variant-add', CAT, '/administration/ProductVariantAdd.aspx?ProductId=', 'a second variant', 'Nop_ProductVariant', ADM)
op('admin.product-copy', CAT, '/administration/ProductDetails.aspx', 'Copy product (its dialog: name, published, images)', 'the copy', ADM)
op('admin.product-delete', CAT, '/administration/ProductDetails.aspx', 'the copy deleted', 'Deleted', ADM)
op('admin.products-search', CAT, '/administration/products.aspx', 'the products\' search', 'the list', ADM)
op('admin.products-go-to-sku', CAT, '/administration/products.aspx', 'Go directly to product SKU', 'the variant\'s page', ADM)
op('admin.bulk-edit-search', CAT, '/administration/bulkeditproducts.aspx', 'the bulk edit\'s search', 'the variants', ADM)
op('admin.bulk-edit-update', CAT, '/administration/bulkeditproducts.aspx', 'a price updated in bulk', 'the price', ADM)
op('admin.reviews', CAT, '/administration/productreviews.aspx', 'the reviews', 'the customer\'s review', ADM)
op('admin.review-approve', CAT, '/administration/ProductReviewDetails.aspx', 'approved, its title edited', 'IsApproved', ADM)
op('admin.review-storefront', CAT, '/products/45-...', 'the approved review on the product\'s page', 'the review', A)
op('admin.tags-delete', CAT, '/administration/producttags.aspx', 'a tag deleted', 'Nop_ProductTag', ADM)
op('admin.low-stock', CAT, '/administration/productvariantslowstock.aspx', 'the low stock report after the orders', 'the variants', ADM)

# --- administration: sales, customers, content, configuration, system ---
SAL, CUS, CON, CFG, SYS = 'admin-sales', 'admin-customers', 'admin-content', 'admin-configuration', 'admin-system'
op('admin.order-details', SAL, '/administration/OrderDetails.aspx', 'the order\'s details', 'its tabs', ADM)
op('admin.order-tabs', SAL, '/administration/OrderDetails.aspx', 'the order\'s tabs', 'each tab', ADM)
op('admin.order-invoice-pdf', SAL, '/administration/OrderDetails.aspx', 'Invoice (PDF)', 'a PDF', ADM)
op('admin.order-packaging-slip-pdf', SAL, '/administration/OrderDetails.aspx', 'Print packaging slip', 'a PDF', ADM)
op('admin.order-mark-paid', SAL, '/administration/OrderDetails.aspx', 'Mark as paid (confirmation)', 'PaymentStatus paid; the gift card bought; the download activated', ADM)
op('admin.order-tracking-number', SAL, '/administration/OrderDetails.aspx', 'the tracking number', 'saved', ADM)
op('admin.order-shipped', SAL, '/administration/OrderDetails.aspx', 'Set as shipped', 'ShippingStatus shipped; the e-mail queued', ADM, partial=SMTP)
op('admin.order-delivered', SAL, '/administration/OrderDetails.aspx', 'Set as delivered', 'complete; the reward points earned', ADM)
op('admin.order-note-add', SAL, '/administration/OrderDetails.aspx', 'an order note for the customer', 'Nop_OrderNote', ADM)
op('admin.orders-search', SAL, '/administration/orders.aspx', 'the orders\' search by the customer\'s e-mail', 'the orders', ADM)
op('admin.orders-go-to-number', SAL, '/administration/orders.aspx', 'Go directly to order number', 'the order', ADM)
op('admin.gift-cards', SAL, '/administration/purchasedgiftcards.aspx', 'the gift cards bought', 'the card', ADM)
op('admin.gift-card-activate', SAL, '/administration/GiftCardDetails.aspx', 'activated', 'IsGiftCardActivated', ADM)
op('admin.gift-card-notify', SAL, '/administration/GiftCardDetails.aspx', 'Notify recipient', 'the e-mail queued', ADM, partial=SMTP)
op('admin.returns', SAL, '/administration/returnrequests.aspx', 'the return requests', 'the request', ADM)
op('admin.return-details', SAL, '/administration/ReturnRequestDetails.aspx', 'a request\'s details', 'the page', ADM)
op('admin.return-edit', SAL, '/administration/ReturnRequestDetails.aspx', 'its status and notes saved', 'Nop_ReturnRequest', ADM)
op('admin.current-carts', SAL, '/administration/currentshoppingcarts.aspx', 'the current carts after the scenario', 'the carts', ADM)
op('admin.sales-report', SAL, '/administration/salesreport.aspx', 'Run report', 'the sales', ADM)
op('admin.customer-details', CUS, '/administration/CustomerDetails.aspx', 'the customer\'s details', 'the tabs', ADM)
op('admin.customer-comment', CUS, '/administration/CustomerDetails.aspx', 'the admin comment saved', 'saved', ADM)
op('admin.customer-tabs', CUS, '/administration/CustomerDetails.aspx', 'roles, addresses, orders, cart, wishlist, place order tabs', 'each tab', ADM)
op('admin.customer-reward-points', CUS, '/administration/CustomerDetails.aspx', 'reward points added', 'Nop_RewardPointsHistory', ADM)
op('admin.customer-send-email', CUS, '/administration/CustomerDetails.aspx', 'an e-mail to the customer', 'queued', ADM, partial=SMTP + '; its body is FCKeditor\'s (left empty)')
op('admin.customers-search', CUS, '/administration/customers.aspx', 'the customers\' search', 'the customers', ADM)
op('admin.customer-impersonate', CUS, '/administration/CustomerDetails.aspx', 'Place order: the store as the customer', 'impersonated', ADM)
op('admin.customer-impersonate-finish', CUS, 'every page', 'Finish session', 'the administrator again', ADM)
op('admin.customer-report', CUS, '/administration/customerreports.aspx', 'customers by order total', 'the report', ADM)
op('admin.online-customers', CUS, '/administration/onlinecustomers.aspx', 'the online customers', 'the page', ADM)
op('admin.blog-comments', CON, '/administration/blogcomments.aspx', 'the blog comments', 'the comment', ADM)
op('admin.blog-comment-edit', CON, '/administration/BlogCommentDetails.aspx', 'a comment\'s details', 'the page', ADM)
op('admin.blog-comment-delete', CON, '/administration/blogcomments.aspx', 'a comment deleted', 'removed', ADM)
op('admin.news-comments', CON, '/administration/newscomments.aspx', 'the news comments', 'the comment', ADM)
op('admin.news-comment-edit', CON, '/administration/NewsCommentDetails.aspx', 'a comment\'s details', 'the page', ADM)
op('admin.news-comment-delete', CON, '/administration/newscomments.aspx', 'a comment deleted', 'removed', ADM)
op('admin.message-template-edit', CON, '/administration/MessageTemplateDetails.aspx', 'a template\'s subject saved', 'Nop_MessageTemplateLocalized', ADM)
op('admin.resource-edit', CON, '/administration/LocaleStringResourceDetails.aspx', 'a resource\'s value saved', 'the store\'s text (account activation\'s title)', ADM)
op('admin.resources-search', CON, '/administration/localestringresources.aspx', 'the resources\' search', 'the resources', ADM)
op('admin.newsletter-search', CON, '/administration/newslettersubscribers.aspx', 'the subscribers', 'the list', ADM)
op('admin.fck-file-browser', CON, '/editors/fckeditor/editor/filemanager/connectors/aspx/connector.aspx', 'FCKeditor\'s file browser connector (administrators only)', 'refused anonymous; the folders and files', ADM,
   keys=['GET /editors/*', 'API * /editors/*'])
op('admin.fck-upload', CON, '/editors/fckeditor/editor/filemanager/connectors/aspx/upload.aspx', 'FCKeditor\'s quick upload',
   'the original\'s own error (500 "Using themed css files requires a header control on the page": the store\'s theme on FCKeditor\'s page, which has no head); no file in images/image', ADM)
op('admin.global-settings-tabs', CFG, '/administration/globalsettings.aspx', 'the global settings\' tabs', 'each tab', ADM)
op('admin.settings-global', CFG, '/administration/globalsettings.aspx', 'avatars, profiles, private messages, the theme selector, anonymous reviews turned on', 'Nop_Setting', ADM)
op('admin.settings-forums', CFG, '/administration/forumssettings.aspx', 'forums turned on, customers allowed to edit / delete / subscribe', 'Nop_Setting', ADM)
op('admin.settings-checkout', CFG, '/administration/globalsettings.aspx', 'anonymous checkout, one-page checkout, terms of service', 'Nop_Setting', ADM)
op('admin.settings-store-closed', CFG, '/administration/globalsettings.aspx', 'the store closed, then opened', 'Nop_Setting', ADM)
op('admin.settings-captcha', CFG, '/administration/globalsettings.aspx', 'the login and registration captcha on, then off', 'Nop_Setting', ADM)
op('admin.settings-registration', CFG, '/administration/globalsettings.aspx', 'registration modes (e-mail validation, disabled, standard), usernames', 'Nop_Setting', ADM)
for page in ('blogsettings', 'newssettings', 'shippingsettings', 'taxsettings', 'paymentmethods', 'shippingmethods', 'taxproviders', 'currencies', 'measures',
             'emailaccounts', 'activitytypes', 'acl', 'smsproviders', 'thirdpartyintegration', 'promotionproviders', 'onlinecustomers', 'forumssettings', 'globalsettings'):
    op(f'admin.settings-save-{page}', CFG, f'/administration/{page}.aspx', 'Save, nothing changed', 'the settings as they were (Nop_Setting compared after)', ADM)
op('admin.email-account-test', CFG, '/administration/EmailAccountDetails.aspx', 'Send test e-mail', 'the SMTP error shown', ADM, partial='the SMTP server is 127.0.0.1:9: the error is compared')
op('admin.currency-live-rates', CFG, '/administration/currencies.aspx', 'Get live rates (ECB)', 'the rates', ADM, runnable=False, reason='asks the European Central Bank\'s site on the Internet')
op('admin.sms-test', CFG, '/administration/smsproviders.aspx', 'Clickatell / Verizon test message', 'an SMS', ADM, runnable=False, reason='the SMS providers are services on the Internet (their accounts)')
op('admin.quickbooks-sync', CFG, '/administration/thirdpartyintegration.aspx', 'QuickBooks synchronize', 'QuickBooks', ADM, runnable=False, reason='needs a QuickBooks installation and its Web Connector')
op('admin.froogle-ftp', CFG, '/administration/promotionproviders.aspx', 'Froogle FTP upload', 'the feed on Google\'s FTP server', ADM, runnable=False, reason='uploads to Google\'s FTP server on the Internet')
op('admin.froogle-generate', CFG, '/administration/promotionproviders.aspx', 'Froogle: Generate feed (files/froogle)', 'the message with the file\'s link', ADM)
op('admin.pricelists', CFG, '/administration/pricelist.aspx', 'the price lists', 'the list', ADM)
for name, label in [('export-products-xml', 'products to XML'), ('export-products-pdf', 'the products\' catalog (PDF)'), ('export-orders-xml', 'orders to XML'),
                    ('export-orders-packaging-slips', 'the orders\' packaging slips (PDF)'), ('export-customers-xml', 'customers to XML'),
                    ('export-categories-xml', 'categories to XML'), ('export-manufacturers-xml', 'manufacturers to XML'), ('export-language-xml', 'a language\'s resources to XML'),
                    ('export-newsletter-csv', 'the newsletter\'s e-mails (CSV, its dialog)')]:
    op('admin.' + name, CFG, 'the list\'s export button', f'export {label}', 'the downloaded file (its content; a PDF\'s pages)', ADM)
for name, label in [('export-products-xls', 'products'), ('export-orders-xls', 'orders'), ('export-customers-xls', 'customers')]:
    op('admin.' + name, CFG, 'the list\'s Excel export button', f'export {label} to Excel (OLE DB, Microsoft.Jet.OLEDB.4.0)', 'the provider\'s error shown (Jet is 32-bit only: the 64-bit process has none)', ADM)
op('admin.import-xls', CFG, '/administration/products.aspx, customers.aspx', 'Import from Excel', 'the products / customers', ADM, runnable=False,
   reason='through OLE DB\'s Jet provider, which a 64-bit process does not have (the original\'s exports fail the same way: admin.export-*-xls); not run')
op('admin.import-newsletter-csv', CFG, '/administration/newslettersubscribers.aspx', 'Import e-mails from CSV (the file chosen in the confirmation\'s modal popup, its OK posting the form)',
   'the count imported; the subscriptions added and updated', ADM)
op('admin.maintenance-backup', SYS, '/administration/maintenance.aspx', 'Backup database (SQL Server writes the .bak into the site\'s folder)', 'the backup listed', ADM, runnable=False,
   reason='SQL Server writes the file itself: its service account needs write access to the application\'s folder (on Linux SQL Server runs in another container: no shared folder)')
op('admin.maintenance-delete-exported', SYS, '/administration/maintenance.aspx', 'Delete old exported files', 'the message', ADM)
op('admin.clear-cache', SYS, 'the administration\'s header', 'Clear Cache', 'the dashboard', ADM)
op('admin.logs', SYS, '/administration/logs.aspx', 'the log', 'the entries', ADM)
op('admin.logs-search', SYS, '/administration/logs.aspx', 'the log searched by a message', 'the entry', ADM)
op('admin.log-details', SYS, '/administration/LogDetails.aspx', 'an entry\'s details', 'the page', ADM)
op('admin.logs-clear', SYS, '/administration/logs.aspx', 'Clear Log', 'empty', ADM)
op('admin.activity-log-search', SYS, '/administration/activitylog.aspx', 'the activity log', 'the entries', ADM)
op('admin.activity-log-clear', SYS, '/administration/activitylog.aspx', 'Clear', 'empty', ADM)
op('admin.message-queue-load', SYS, '/administration/messagequeue.aspx', 'the queued e-mails (all)', 'the e-mails', ADM)
op('admin.message-queue-details', SYS, '/administration/MessageQueueDetails.aspx', 'an e-mail', 'the page', ADM)
op('admin.message-queue-requeue', SYS, '/administration/MessageQueueDetails.aspx', 'Requeue', 'a copy queued', ADM)
op('admin.message-queue-delete', SYS, '/administration/MessageQueueDetails.aspx', 'Delete', 'removed', ADM)
op('admin.activity-log', SYS, 'the activity log', 'the administrator\'s activities (Nop_ActivityLog)', 'the entries', ADM)

here = os.path.dirname(os.path.abspath(__file__))
ids = [o['id'] for o in OPS]
dup = [i for i in ids if ids.count(i) > 1]
assert not dup, f'duplicate ids: {sorted(set(dup))}'
with open(os.path.join(here, 'nop.operations.json'), 'w', encoding='utf-8', newline='\n') as f:
    json.dump({'_comment': 'Generated by nop-operations-inventory.py (edit the table there). The operations of nopCommerce 1.90; corpora/regression/nop.operations.scenario.json exercises them (its steps\' op).',
               'operations': OPS}, f, ensure_ascii=False, indent=2)
    f.write('\n')

cats = {}
for o in OPS:
    cats.setdefault(o['category'], []).append(o)
lines = ['# nopCommerce 1.90: the inventory of operations', '',
         'Generated by `nop-operations-inventory.py` from the same table as `nop.operations.json` (edit the table there and run it).',
         'Each operation is exercised by the steps of `nop.operations.scenario.json` that name it (`op`); the golden data is recorded on the',
         'original on IIS (`corpora/record-nop-operations.ps1`) and compared on Windows and Linux (`verify-windows.ps1 -Name nop -Suite operations`,',
         '`run-linux.ps1 ... -Suite operations`). Every run starts from the sample store\'s database (`corpora/nop-database.ps1`).', '',
         '| category | operations | not runnable | partial |', '|---|---|---|---|']
for c, items in cats.items():
    lines.append(f'| {c} | {len(items)} | {sum(1 for i in items if not i["runnable"])} | {sum(1 for i in items if i.get("partial"))} |')
lines.append(f'| **total** | **{len(OPS)}** | **{sum(1 for i in OPS if not i["runnable"])}** | **{sum(1 for i in OPS if i.get("partial"))}** |')
for c, items in cats.items():
    lines += ['', f'## {c}', '', '| id | page | element | role / preconditions | expected |', '|---|---|---|---|---|']
    for o in items:
        role = o.get('role', '') + ('; ' + o['preconditions'] if o.get('preconditions') else '')
        exp = o.get('expected', '')
        if not o['runnable']:
            exp = f'**not runnable**: {o["reason"]}'
        elif o.get('partial'):
            exp += f' (partial: {o["partial"]})'
        cells = [o['id'], o['page'], o['element'], role, exp]
        lines.append('| ' + ' | '.join(str(x).replace('|', '\\|') for x in cells) + ' |')
with open(os.path.join(here, 'nop.operations.md'), 'w', encoding='utf-8', newline='\n') as f:
    f.write('\n'.join(lines) + '\n')
print(len(OPS), 'operations,', sum(1 for o in OPS if not o['runnable']), 'not runnable')
