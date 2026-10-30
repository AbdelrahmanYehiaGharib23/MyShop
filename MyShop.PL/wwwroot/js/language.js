(function () {
    'use strict';

    var key = 'myshop-language';
    var englishToArabic = {
        'Skip to content': 'تخطَّ إلى المحتوى', 'Secure checkout': 'دفع آمن', 'Clear order summary': 'ملخص طلب واضح', 'Order status in your account': 'حالة الطلب في حسابك',
        'Products': 'المنتجات', 'Favorites': 'المفضلة', 'Cart': 'السلة', 'Orders': 'الطلبات', 'Search products...': 'ابحث عن المنتجات...', 'Search products': 'ابحث عن المنتجات', 'Search': 'بحث',
        'Customer account': 'حساب العميل', 'My Orders': 'طلباتي', 'Logout': 'تسجيل الخروج', 'Login': 'تسجيل الدخول', 'Register': 'إنشاء حساب', 'Dark Mode': 'الوضع الداكن', 'Light Mode': 'الوضع الفاتح', 'Back': 'رجوع',
        'Browse selected tech products, save favorites and manage your orders in one place.': 'تصفّح منتجات تقنية مختارة، واحفظ المفضلة، وتابع طلباتك من مكان واحد.',
        'Shop': 'تسوّق', 'Categories': 'التصنيفات', 'All products': 'كل المنتجات', 'Secure payments': 'دفع آمن', 'Choose an available payment method during checkout.': 'اختر وسيلة الدفع المتاحة عند إتمام الطلب.', 'Payments': 'وسائل الدفع',
        'Customer Storefront': 'متجر العملاء', 'Shop the latest tech for work and everyday life.': 'اكتشف أحدث المنتجات التقنية للعمل والحياة اليومية.', 'Discover selected products, compare prices at a glance, and add what you need to your cart.': 'اكتشف منتجات مختارة، وقارن الأسعار، وأضف احتياجاتك إلى السلة.',
        'Shop Products': 'تسوّق المنتجات', 'View Cart': 'عرض السلة', 'Clear pricing': 'أسعار واضحة', 'Secure Checkout': 'إتمام دفع آمن', 'Account order history': 'سجل الطلبات في الحساب', 'Saved favorites': 'منتجات محفوظة في المفضلة',
        'Catalog': 'الكتالوج', 'Featured Products': 'منتجات مميزة', 'Fresh picks from your store catalog.': 'مجموعة مختارة من منتجات المتجر.', 'Sort': 'ترتيب', 'Show': 'عرض', 'Featured': 'مميزة', 'Name: A to Z': 'الاسم: من أ إلى ي', 'Name: Z to A': 'الاسم: من ي إلى أ', 'Price: Low to High': 'السعر: من الأقل للأعلى', 'Price: High to Low': 'السعر: من الأعلى للأقل', 'Sort products': 'ترتيب المنتجات', 'Products per page': 'عدد المنتجات في الصفحة', 'Clear Search': 'مسح البحث', 'Open Cart': 'فتح السلة',
        'Total products': 'إجمالي المنتجات', 'Average price': 'متوسط السعر', 'No reviews': 'لا توجد مراجعات', 'No reviews yet': 'لا توجد مراجعات بعد', 'Add to Cart': 'أضف إلى السلة', 'View Details': 'عرض التفاصيل', 'Previous': 'السابق', 'Next': 'التالي', 'Explore this product in our catalog.': 'اكتشف هذا المنتج في الكتالوج.',
        'Shopping Cart': 'سلة التسوق', 'Your Cart': 'سلتك', 'Review your selected products before checkout.': 'راجع المنتجات التي اخترتها قبل إتمام الطلب.', 'Continue Shopping': 'متابعة التسوق', 'Unit price:': 'سعر الوحدة:', 'Line total': 'إجمالي المنتج', 'Decrease quantity': 'تقليل الكمية', 'Increase quantity': 'زيادة الكمية', 'Remove item': 'حذف المنتج', 'Order Summary': 'ملخص الطلب', 'Coupon code': 'رمز الخصم', 'Apply': 'تطبيق', 'Total items': 'عدد المنتجات', 'Subtotal': 'المجموع الفرعي', 'Shipping': 'الشحن', 'No fee added': 'لا توجد رسوم مضافة', 'The displayed total includes product prices only.': 'الإجمالي المعروض يشمل أسعار المنتجات فقط.', 'Keep Shopping': 'متابعة التسوق', 'Clear Cart': 'إفراغ السلة', 'Your cart is empty': 'سلتك فارغة', 'Products you add to the cart will appear here with quantity, price, and order summary.': 'ستظهر هنا المنتجات التي تضيفها إلى السلة مع الكمية والسعر وملخص الطلب.', 'Browse Products': 'تصفّح المنتجات',
        'Checkout': 'إتمام الطلب', 'Pay securely with Stripe.': 'ادفع بأمان عبر Stripe.', 'Choose how to pay.': 'اختر طريقة الدفع.', 'Confirm your delivery details.': 'أكّد تفاصيل التوصيل.', 'Pay securely with Paymob.': 'ادفع بأمان عبر Paymob.', 'Your card details are encrypted and handled directly by Stripe.': 'بيانات بطاقتك مشفرة ويعالجها Stripe مباشرةً.', 'Your card details are encrypted and handled directly by Paymob.': 'بيانات بطاقتك مشفرة ويعالجها Paymob مباشرةً.', 'Select a payment provider to continue to its secure checkout.': 'اختر مزوّد الدفع للمتابعة إلى صفحة الدفع الآمنة.', 'Add the address and phone number for this order. Your cart items will be converted into a saved customer order.': 'أضف عنوان التوصيل ورقم الهاتف. ستتحول منتجات السلة إلى طلب محفوظ في حسابك.', 'Delivery': 'التوصيل', 'Payment': 'الدفع', 'Return to Cart': 'العودة إلى السلة', 'Continue shopping': 'متابعة التسوق', 'Delivery details': 'تفاصيل التوصيل', 'Shipping address': 'عنوان التوصيل', 'Phone number': 'رقم الهاتف', 'Street, building, apartment, city': 'الشارع، المبنى، الشقة، المدينة', 'Payment method': 'وسيلة الدفع', 'Pay with Stripe': 'الدفع عبر Stripe', 'Pay with Paymob': 'الدفع عبر Paymob', 'Place Order': 'تأكيد الطلب', 'Stripe charge': 'المبلغ عبر Stripe', 'Paymob charge': 'المبلغ عبر Paymob', 'Order placed': 'تم إنشاء الطلب', 'Delivery comes first': 'ابدأ بتفاصيل التوصيل', 'Save your delivery details to see the available payment providers for this order.': 'احفظ بيانات التوصيل لعرض وسائل الدفع المتاحة لهذا الطلب.', 'Select a payment provider': 'اختر وسيلة الدفع', 'Paymob charges in EGP; Stripe charges in USD.': 'يعالج Paymob الدفع بالجنيه المصري، بينما يعالجه Stripe بالدولار الأمريكي.', 'Pay by card': 'الدفع بالبطاقة', 'Card payment in US dollars through Stripe.': 'دفع بالبطاقة بالدولار الأمريكي عبر Stripe.', 'Continue with Stripe': 'المتابعة عبر Stripe', 'Stripe keys are not configured.': 'مفاتيح Stripe غير معدّة.', 'Card payment in Egyptian pounds through Paymob.': 'دفع بالبطاقة بالجنيه المصري عبر Paymob.', 'Pay securely with Paymob': 'الدفع بأمان عبر Paymob', 'All payment methods': 'كل وسائل الدفع', 'Card payment': 'دفع بالبطاقة', 'Card Details': 'بيانات البطاقة', 'Secure card payment': 'دفع آمن بالبطاقة', 'Preparing Paymob’s secure card form...': 'جارٍ تجهيز نموذج Paymob الآمن...', 'Preparing Paymob\'s secure card form...': 'جارٍ تجهيز نموذج Paymob الآمن...', 'Your card details are sent directly to Paymob. Your order is confirmed after payment verification.': 'تُرسل بيانات بطاقتك مباشرةً إلى Paymob، ويُؤكد طلبك بعد التحقق من الدفع.', 'Your card details are sent directly to Stripe. Your order is confirmed after payment verification.': 'تُرسل بيانات بطاقتك مباشرةً إلى Stripe، ويُؤكد طلبك بعد التحقق من الدفع.', 'Card number': 'رقم البطاقة', 'Expiry': 'تاريخ الانتهاء', 'Delivery phone': 'هاتف التوصيل', 'Delivery address': 'عنوان التوصيل', 'No fee added': 'لا توجد رسوم مضافة',
        'Saved favorites': 'المفضلة المحفوظة', 'YOUR COLLECTION': 'مجموعتك', 'Keep the products you love close, then pick up right where you left off.': 'احتفظ بالمنتجات التي أعجبتك وعد إليها وقتما تشاء.', 'Your favorites start here': 'ابدأ قائمتك من هنا', 'Tap the heart on any product to save it to this list.': 'اضغط على القلب بجوار أي منتج لإضافته إلى هذه القائمة.', 'Explore products': 'استكشف المنتجات',
        'Price': 'السعر', 'Product details will be available soon.': 'ستتوفر تفاصيل المنتج قريبًا.', 'No customer rating yet': 'لا توجد تقييمات بعد', 'Customer Reviews': 'مراجعات العملاء', 'Share your experience and help other customers choose with confidence.': 'شارك تجربتك وساعد العملاء الآخرين على الاختيار.', 'Write a review': 'اكتب مراجعة', 'Rate the product and leave a helpful note for other shoppers.': 'قيّم المنتج واكتب ملاحظة مفيدة للمتسوقين.', 'Rating': 'التقييم', 'Your review': 'مراجعتك', 'What stood out about this product?': 'ما أكثر ما أعجبك في هذا المنتج؟', 'Delete': 'حذف', 'Submit Review': 'إرسال المراجعة', 'Login to write a review': 'سجّل الدخول لكتابة مراجعة', 'Login to Review': 'سجّل الدخول للمراجعة', 'Latest reviews': 'أحدث المراجعات', 'Loading customer feedback...': 'جارٍ تحميل آراء العملاء...', 'Loading': 'جارٍ التحميل', 'Be the first customer to share a rating for this product.': 'كن أول من يقيّم هذا المنتج.',
        'My Orders': 'طلباتي', 'Order date': 'تاريخ الطلب', 'Items': 'المنتجات', 'Phone': 'الهاتف', 'Delivery address': 'عنوان التوصيل', 'Delivery journey': 'مراحل التوصيل', 'Delivery paused': 'التوصيل متوقف', 'Planned delivery route': 'مسار التوصيل', 'Order processing': 'جارٍ تجهيز الطلب', 'Payment confirmed': 'تم تأكيد الدفع', 'Waiting for payment': 'بانتظار الدفع', 'Payment has been confirmed for this order.': 'تم تأكيد الدفع لهذا الطلب.', 'No delivery updates are available for this order.': 'لا توجد تحديثات توصيل لهذا الطلب.', 'Delivery preparation starts after payment succeeds.': 'يبدأ تجهيز الطلب بعد نجاح الدفع.', 'Preparation': 'التجهيز', 'This order will not proceed to delivery.': 'لن ينتقل هذا الطلب إلى مرحلة التوصيل.', 'Starts after payment confirmation.': 'يبدأ بعد تأكيد الدفع.', 'Your order is waiting to be prepared.': 'طلبك بانتظار التجهيز.', 'Your order is being prepared.': 'يجري تجهيز طلبك.', 'Your order has been prepared.': 'تم تجهيز طلبك.', 'With the courier': 'مع مندوب التوصيل', 'Courier delivery is unavailable.': 'التوصيل عبر المندوب غير متاح.', 'Assigned': 'تم التعيين', 'Not provided': 'غير متوفر', 'Courier and tracking details will appear after dispatch.': 'ستظهر بيانات المندوب والتتبع بعد الإرسال.', 'Delivered': 'تم التوصيل', 'Delivery is paused for this order.': 'التوصيل متوقف لهذا الطلب.', 'Your order has been delivered.': 'تم توصيل طلبك.', 'Delivery confirmation will appear here.': 'سيظهر تأكيد التوصيل هنا.', 'Estimated arrival': 'موعد الوصول المتوقع', 'Added by the store after dispatch': 'يضيفه المتجر بعد الإرسال', 'Courier location': 'موقع مندوب التوصيل', 'Live location is not available.': 'الموقع المباشر غير متاح.', 'Courier details appear after dispatch.': 'تظهر بيانات المندوب بعد الإرسال.', 'This page shows order stages and the courier details entered by the store. Live courier location requires a separate tracking integration.': 'تعرض هذه الصفحة مراحل الطلب وبيانات المندوب التي أضافها المتجر. يتطلب الموقع المباشر تكاملًا منفصلًا للتتبع.', 'Order items': 'منتجات الطلب', 'Show more items': 'عرض منتجات إضافية', 'No products are attached to this order yet.': 'لا توجد منتجات مرتبطة بهذا الطلب بعد.', 'No orders in this status': 'لا توجد طلبات بهذه الحالة', 'Choose another status to see your orders.': 'اختر حالة أخرى لعرض طلباتك.', 'No orders yet': 'لا توجد طلبات بعد', 'When you place an order from checkout, it will appear here with its products and delivery details.': 'عند تأكيد طلبك سيظهر هنا مع منتجاته وتفاصيل التوصيل.', 'Start Shopping': 'ابدأ التسوق',
        'Register': 'إنشاء حساب', 'Login': 'تسجيل الدخول', 'Forgot Password': 'نسيت كلمة المرور', 'Reset Password': 'إعادة تعيين كلمة المرور', 'Verify OTP': 'تأكيد رمز التحقق', 'Access Denied': 'تم رفض الوصول', 'Enter your first name': 'أدخل اسمك الأول', 'Enter your last name': 'أدخل اسم العائلة', 'Choose a username': 'اختر اسم مستخدم', 'Create a password': 'أنشئ كلمة مرور', 'Enter your password again': 'أعد إدخال كلمة المرور', 'Enter your password': 'أدخل كلمة المرور', 'you@example.com': 'name@example.com', 'Email': 'البريد الإلكتروني', 'Email address': 'البريد الإلكتروني', 'Password': 'كلمة المرور', 'Confirm Password': 'تأكيد كلمة المرور', 'Confirm password': 'تأكيد كلمة المرور', 'First name': 'الاسم الأول', 'First Name': 'الاسم الأول', 'Last name': 'اسم العائلة', 'Last Name': 'اسم العائلة', 'Username': 'اسم المستخدم', 'Remember me': 'تذكرني', 'Remember Me': 'تذكرني', 'Already have an account?': 'لديك حساب بالفعل؟', 'Don\'t have an account?': 'ليس لديك حساب؟', 'Create Account': 'إنشاء حساب', 'Sign in': 'دخول', 'SEND OTP': 'إرسال رمز التحقق', 'VERIFY OTP': 'تأكيد رمز التحقق', 'RESET PASSWORD': 'تغيير كلمة المرور', 'New password': 'كلمة المرور الجديدة', 'OTP Code': 'رمز التحقق', 'Verification code': 'رمز التحقق', 'Welcome back': 'مرحبًا بعودتك', 'Sign in to continue shopping and track your orders.': 'سجّل الدخول لمتابعة التسوق وتتبع طلباتك.', 'Email address': 'البريد الإلكتروني', 'Forgot Password?': 'نسيت كلمة المرور؟', 'Don\'t have an account?': 'ليس لديك حساب؟', 'Register Now!': 'أنشئ حسابًا الآن', 'Create your account': 'أنشئ حسابك', 'A few details to get your shopping started.': 'أدخل بعض البيانات لتبدأ التسوق.', 'First name': 'الاسم الأول', 'Last name': 'اسم العائلة', 'I Agree To The Terms & Conditions': 'أوافق على الشروط والأحكام', 'SIGN UP': 'إنشاء حساب', 'Login Now!': 'سجّل الدخول الآن', 'Forgot Password': 'نسيت كلمة المرور', 'Enter your email and we will send you a verification code.': 'أدخل بريدك الإلكتروني وسنرسل إليك رمز تحقق.', 'SEND OTP': 'إرسال رمز التحقق', 'Remembered your password?': 'تذكرت كلمة المرور؟', 'Verify OTP': 'تأكيد رمز التحقق', 'Use the code sent to your email to continue.': 'استخدم الرمز المرسل إلى بريدك الإلكتروني للمتابعة.', 'VERIFY OTP': 'تأكيد رمز التحقق', 'Back to': 'العودة إلى', 'Reset Password': 'إعادة تعيين كلمة المرور', 'Create a new password for your myShop account.': 'أنشئ كلمة مرور جديدة لحسابك في myShop.', 'RESET PASSWORD': 'تغيير كلمة المرور', 'Welcome to myShop': 'مرحبًا بك في myShop', 'YOUR EVERYDAY TECH STORE': 'متجرك اليومي للتقنية', 'Good finds.': 'اختيارات مميزة.', 'A smoother day.': 'ليوم أسهل.', 'Discover useful tech, save your favorites, and keep every order in one simple place.': 'اكتشف منتجات تقنية مفيدة، واحفظ المفضلة، وتابع طلباتك من مكان واحد.', 'SMART PICKS': 'اختيارات ذكية', 'Secure checkout · Curated products · Made for everyday': 'دفع آمن · منتجات مختارة · لاحتياجاتك اليومية',
        'Could not update your favorites.': 'تعذر تحديث المفضلة.', 'Choose a valid product.': 'اختر منتجًا صالحًا.', 'This product is no longer available.': 'هذا المنتج لم يعد متاحًا.', 'Favorites could not be loaded': 'تعذر تحميل المفضلة', 'Could not update favorites': 'تعذر تحديث المفضلة', 'Saved to favorites': 'أُضيف إلى المفضلة', 'Removed from favorites': 'أُزيل من المفضلة', 'Could not save favorite': 'تعذر حفظ المفضلة', 'Allow site storage and try again.': 'اسمح بحفظ البيانات في المتصفح ثم حاول مجددًا.', 'No fee added': 'لا توجد رسوم مضافة'
    };

    Object.assign(englishToArabic, {
        'Payment Status': 'حالة الدفع',
        'Order payment': 'دفع الطلب',
        'We’re verifying your payment': 'جارٍ التحقق من الدفع',
        'The payment provider is confirming your transaction. This can take a few moments.': 'يتحقق مزوّد الدفع من معاملتك. قد يستغرق ذلك لحظات.',
        'Your payment has been verified and your order is confirmed.': 'تم التحقق من الدفع وتأكيد طلبك.',
        'We’ll check for an update automatically. You can also check again manually.': 'سنتحقق من الحالة تلقائيًا. ويمكنك أيضًا إعادة التحقق يدويًا.',
        'Payment is still pending. Check again in a moment or return to payment.': 'لا يزال الدفع قيد التحقق. أعد المحاولة بعد لحظات أو عُد إلى الدفع.',
        'Could not reach the order status service. Check your connection and try again.': 'تعذر الوصول إلى خدمة حالة الطلب. تحقق من اتصالك وحاول مجددًا.',
        'Payment was not confirmed': 'لم يتم تأكيد الدفع',
        'Your payment was not confirmed. You can review the order status in your account.': 'لم يتم تأكيد الدفع. يمكنك مراجعة حالة الطلب في حسابك.',
        'This order was cancelled': 'تم إلغاء هذا الطلب',
        'This order is no longer available for payment.': 'لم يعد هذا الطلب متاحًا للدفع.',
        'If you need help with this order, check its details in My Orders.': 'إذا احتجت إلى مساعدة بشأن الطلب، راجع تفاصيله في صفحة طلباتي.',
        'You can view delivery updates and order details in My Orders.': 'يمكنك متابعة تحديثات التوصيل وتفاصيل الطلب في صفحة طلباتي.',
        'Check status': 'تحقق من الحالة',
        'Return to payment': 'العودة إلى الدفع',
        'View my orders': 'عرض طلباتي',
        'Continue shopping': 'متابعة التسوق',
        'Page not found': 'الصفحة غير موجودة',
        'Access unavailable': 'الوصول غير متاح',
        'Something went wrong': 'حدث خطأ ما',
        'We couldn’t find the page you requested. It may have moved or the address may be incorrect.': 'لم نعثر على الصفحة المطلوبة. ربما تغير مكانها أو أن العنوان غير صحيح.',
        'You don’t have access to this page. Return to the store or sign in with the right account.': 'ليس لديك صلاحية الوصول إلى هذه الصفحة. عُد إلى المتجر أو سجّل الدخول بالحساب الصحيح.',
        'We couldn’t complete that request. Please try again in a moment.': 'تعذر إكمال الطلب. حاول مرة أخرى بعد قليل.',
        'Reference': 'المرجع',
        'Return to the store': 'العودة إلى المتجر',
        'Sign in': 'تسجيل الدخول',
        'You’re offline. Some store actions may need a connection.': 'أنت غير متصل بالإنترنت. قد تحتاج بعض وظائف المتجر إلى اتصال.',
        'Connection restored. You can continue.': 'عاد الاتصال بالإنترنت. يمكنك المتابعة.',
        'Order': 'الطلب',
        'Total': 'الإجمالي',
        'Could not refresh the order status.': 'تعذر تحديث حالة الطلب.',
        'Could not add item': 'تعذر إضافة المنتج',
        'Add to cart failed. Try again.': 'تعذرت إضافة المنتج إلى السلة. حاول مجددًا.',
        'Choose a valid product quantity.': 'اختر كمية صالحة للمنتج.',
        'Prices changed or a product was removed. Review the updated cart before checkout.': 'تغيّر سعر منتج أو أُزيل من السلة. راجع السلة المحدّثة قبل إتمام الطلب.',
        'Prices changed or a product was removed. Review the updated order total.': 'تغيّر سعر منتج أو أُزيل من السلة. راجع إجمالي الطلب المحدّث.',
        'Prices changed or a product was removed. Review the updated order total and continue again.': 'تغيّر سعر منتج أو أُزيل من السلة. راجع إجمالي الطلب المحدّث ثم تابع مجددًا.',
        'Switch to light theme': 'التبديل إلى الوضع الفاتح',
        'Switch to dark theme': 'التبديل إلى الوضع الداكن',
        'Switch theme': 'تغيير المظهر',
        'Switch language': 'تغيير اللغة',
        'Breadcrumb': 'مسار التنقل',
        'Category': 'التصنيف',
        'Back to Products': 'العودة إلى المنتجات',
        'Review the full order total before payment.': 'راجع إجمالي الطلب قبل الدفع.',
        'Delivery Information': 'بيانات التوصيل',
        'Add the phone number and address where you want this order delivered.': 'أدخل رقم الهاتف والعنوان الذي تريد توصيل طلبك إليه.',
        'Continue to Payment': 'المتابعة إلى الدفع',
        'Card details are processed securely by the provider you choose. Your order is confirmed after payment verification.': 'تُعالج بيانات البطاقة بأمان لدى مزوّد الدفع الذي تختاره، ويُؤكد طلبك بعد التحقق من الدفع.',
        'Preparing Paymob\'s secure card form...': 'جارٍ تجهيز نموذج Paymob الآمن...',
        'Your card details are sent directly to Paymob. Your order is confirmed after payment verification.': 'تُرسل بيانات بطاقتك مباشرةً إلى Paymob، ويُؤكد طلبك بعد التحقق من الدفع.',
        'Your card details are sent directly to Stripe. Your order is confirmed after payment verification.': 'تُرسل بيانات بطاقتك مباشرةً إلى Stripe، ويُؤكد طلبك بعد التحقق من الدفع.',
        'All orders': 'كل الطلبات',
        'Customer Orders': 'طلبات العملاء',
        'Track your saved orders, delivery details, item quantities, and totals from one clean customer page.': 'تابع طلباتك وتفاصيل التوصيل والكميات والإجماليات من صفحة واحدة.',
        'Filter orders by status': 'تصفية الطلبات حسب الحالة',
        'orders': 'طلبات',
        'Pending': 'قيد الانتظار',
        'Paid': 'مدفوع',
        'Failed': 'فشل الدفع',
        'Cancelled': 'ملغي',
        'Order confirmed': 'تم تأكيد الطلب',
        'Waiting for preparation': 'بانتظار التجهيز',
        'Being prepared': 'قيد التجهيز',
        'Courier:': 'المندوب:',
        'Tracking:': 'رقم التتبع:',
        'Free': 'بدون رسوم',
        'No fee added': 'لا توجد رسوم مضافة',
        'Adding...': 'جارٍ الإضافة...',
        'Added': 'تمت الإضافة',
        'Added to cart': 'أُضيف إلى السلة',
        'Could not add item': 'تعذرت إضافة المنتج',
        'Please try adding the product again.': 'حاول إضافة المنتج مجددًا.',
        'Payment loading': 'جارٍ تحميل الدفع',
        'Stripe is preparing the secure payment form.': 'يجهّز Stripe نموذج الدفع الآمن.',
        'Connecting to Paymob...': 'جارٍ الاتصال بـ Paymob...',
        'Paymob unavailable': 'Paymob غير متاح',
        'Could not load product reviews.': 'تعذر تحميل مراجعات المنتج.',
        'Review request failed.': 'تعذر إرسال طلب المراجعة.',
        'Reviews unavailable': 'المراجعات غير متاحة',
        'Choose a rating': 'اختر تقييمًا',
        'Please select between 1 and 5 stars.': 'اختر تقييمًا من نجمة إلى خمس نجوم.',
        'Write a review': 'اكتب مراجعة',
        'Please add a short comment before submitting.': 'أضف تعليقًا قصيرًا قبل الإرسال.',
        'Review updated': 'تم تحديث المراجعة',
        'Review added': 'تمت إضافة المراجعة',
        'Thanks for sharing your product feedback.': 'شكرًا لمشاركة رأيك في المنتج.',
        'Review not saved': 'لم تُحفظ المراجعة',
        'Review deleted': 'تم حذف المراجعة',
        'Your review was removed from this product.': 'أُزيلت مراجعتك من هذا المنتج.',
        'Could not delete review': 'تعذر حذف المراجعة',
        'Update Review': 'تحديث المراجعة',
        'Delete': 'حذف',
        'Submit Review': 'إرسال المراجعة',
        'Loading your saved products…': 'جارٍ تحميل المنتجات المحفوظة…',
        'All rights reserved.': 'جميع الحقوق محفوظة.',
        'Product': 'المنتج',
        'Customer': 'عميل',
        'Choose a valid product.': 'اختر منتجًا صالحًا.',
        'Card expiry date': 'تاريخ انتهاء البطاقة',
        'Card CVC': 'رمز أمان البطاقة',
        'Return to all payment methods': 'العودة إلى كل وسائل الدفع',
        'Back to payment provider selection': 'العودة لاختيار وسيلة الدفع',
        'Pay securely with Stripe': 'الدفع بأمان عبر Stripe',
        'Paymob keys, integration ID, or public HTTPS callback URL are missing.': 'مفاتيح Paymob أو رقم التكامل أو رابط HTTPS للعودة غير مكتمل.',
        'Stripe keys are not configured yet.': 'لم تُعد مفاتيح Stripe بعد.',
        'Add your publishable and secret keys in User Secrets to enable card payment.': 'أضف المفتاحين العام والخاص في User Secrets لتفعيل الدفع بالبطاقة.',
        'Stripe Unavailable': 'Stripe غير متاح',
        'Payment Unavailable': 'الدفع غير متاح',
        'Your cart is empty': 'سلتك فارغة',
        'Add products to your cart before starting checkout.': 'أضف منتجات إلى السلة قبل إتمام الطلب.',
        'Order Summary': 'ملخص الطلب',
        'YOUR COLLECTION': 'مجموعتك',
        'Explore': 'استكشف',
        'Price preview': 'معاينة السعر',
        'Product name': 'اسم المنتج',
        'Terms & Conditions': 'الشروط والأحكام',
        'Main navigation': 'التنقل الرئيسي',
        'Toggle navigation': 'فتح أو إغلاق القائمة',
        'Store benefits': 'مزايا المتجر',
        'Store features': 'ميزات المتجر',
        'Product categories': 'تصنيفات المنتجات',
        'Product pagination': 'صفحات المنتجات',
        'Open cart': 'فتح السلة',
        'Select a payment provider': 'اختر وسيلة الدفع',
        'Decorative Visa card preview': 'معاينة زخرفية لبطاقة Visa',
        'All payment methods': 'كل وسائل الدفع',
        'Card number': 'رقم البطاقة',
        'Decrease quantity': 'تقليل الكمية',
        'Increase quantity': 'زيادة الكمية',
        'Remove item': 'حذف المنتج',
        'Switch to Arabic': 'التبديل إلى العربية',
        'Switch to English': 'التبديل إلى الإنجليزية',
        'Next': 'التالي',
        'Previous': 'السابق',
        'No fee added': 'لا توجد رسوم مضافة'
    });

    var arabicToEnglish = Object.keys(englishToArabic).reduce(function (result, english) {
        var arabic = englishToArabic[english];
        if (arabic && !result[arabic]) result[arabic] = english;
        return result;
    }, {});

    function safeLanguage() {
        try {
            var stored = localStorage.getItem(key);
            if (stored === 'ar' || stored === 'en') return stored;
        } catch (_) { }
        return (navigator.language || '').toLowerCase().indexOf('ar') === 0 ? 'ar' : 'en';
    }

    function translatePattern(value, language) {
        var match;
        if (language === 'ar') {
            match = value.match(/^Search results for “?(.*?)”?$/);
            if (match) return 'نتائج البحث عن «' + match[1] + '»';
            match = value.match(/^Search results for "(.*?)"$/);
            if (match) return 'نتائج البحث عن «' + match[1] + '»';
            match = value.match(/^Showing (\d+)[–-](\d+) of (\d+) products$/);
            if (match) return 'عرض ' + match[1] + '–' + match[2] + ' من ' + match[3] + ' منتج';
            match = value.match(/^(\d+) products? matched your search\.$/);
            if (match) return 'عدد المنتجات المطابقة لبحثك: ' + match[1] + '.';
            match = value.match(/^Rated ([\d.]+) out of 5 from (\d+) reviews?$/);
            if (match) return 'التقييم ' + match[1] + ' من ٥ بناءً على ' + match[2] + ' مراجعة';
            match = value.match(/^Unit price: (.+)$/);
            if (match) return 'سعر الوحدة: ' + match[1];
            match = value.match(/^Explore the (.+) collection\.$/);
            if (match) return 'اكتشف مجموعة ' + match[1] + '.';
            match = value.match(/^Qty (\d+) x (.+)$/);
            if (match) return 'الكمية ' + match[1] + ' × ' + match[2];
            match = value.match(/^Show (\d+) more items$/);
            if (match) return 'عرض ' + match[1] + ' منتجات إضافية';
            match = value.match(/^(\d+) orders?$/);
            if (match) return match[1] + ' طلب';
            match = value.match(/^Order #?(\d+)$/);
            if (match) return 'الطلب رقم ' + match[1];
            match = value.match(/^Order (Pending|Paid|Failed|Cancelled)$/);
            if (match) return 'الطلب ' + (englishToArabic[match[1]] || match[1]);
            match = value.match(/^([\d.]+) customer rating · (\d+) reviews?$/);
            if (match) return 'التقييم ' + match[1] + ' من العملاء · ' + match[2] + ' مراجعة';
            match = value.match(/^Courier: (.+) · Tracking: (.+)$/);
            if (match) return 'المندوب: ' + match[1] + ' · رقم التتبع: ' + match[2];
            match = value.match(/^(.+) is now in your cart\.$/);
            if (match) return 'أُضيف ' + match[1] + ' إلى سلتك.';
            match = value.match(/^Rated ([\d.]+) out of 5 from (\d+) reviews?$/);
            if (match) return 'التقييم ' + match[1] + ' من ٥ بناءً على ' + match[2] + ' مراجعة';
        } else {
            match = value.match(/^نتائج البحث عن «(.*?)»$/);
            if (match) return 'Search results for “' + match[1] + '”';
            match = value.match(/^عرض (\d+)–(\d+) من (\d+) منتج$/);
            if (match) return 'Showing ' + match[1] + '–' + match[2] + ' of ' + match[3] + ' products';
            match = value.match(/^عدد المنتجات المطابقة لبحثك: (\d+)\.$/);
            if (match) return match[1] + ' products matched your search.';
            match = value.match(/^سعر الوحدة: (.+)$/);
            if (match) return 'Unit price: ' + match[1];
            match = value.match(/^الكمية (\d+) × (.+)$/);
            if (match) return 'Qty ' + match[1] + ' x ' + match[2];
            match = value.match(/^اكتشف مجموعة (.+)\.$/);
            if (match) return 'Explore the ' + match[1] + ' collection.';
            match = value.match(/^عرض (\d+) منتجات إضافية$/);
            if (match) return 'Show ' + match[1] + ' more items';
            match = value.match(/^(\d+) طلب$/);
            if (match) return match[1] + ' orders';
            match = value.match(/^الطلب رقم (\d+)$/);
            if (match) return 'Order #' + match[1];
            match = value.match(/^الطلب (قيد الانتظار|مدفوع|فشل الدفع|ملغي)$/);
            if (match) return 'Order ' + (arabicToEnglish[match[1]] || match[1]);
            match = value.match(/^التقييم ([\d.]+) من العملاء · (\d+) مراجعة$/);
            if (match) return match[1] + ' customer rating · ' + match[2] + ' reviews';
            match = value.match(/^المندوب: (.+) · رقم التتبع: (.+)$/);
            if (match) return 'Courier: ' + match[1] + ' · Tracking: ' + match[2];
        }
        return value;
    }

    function localizeValue(value, language) {
        var trimmed = value.trim();
        if (!trimmed) return value;
        var dictionary = language === 'ar' ? englishToArabic : arabicToEnglish;
        var translated = dictionary[trimmed];
        if (translated === undefined) translated = translatePattern(trimmed, language);
        if (translated === trimmed) return value;
        var leading = value.match(/^\s*/)[0];
        var trailing = value.match(/\s*$/)[0];
        return leading + translated + trailing;
    }

    function updateLanguageButtons(language) {
        document.querySelectorAll('[data-language-toggle]').forEach(function (button) {
            var label = language === 'ar' ? 'Switch to English' : 'التبديل إلى العربية';
            var visual = button.querySelector('[aria-hidden="true"]');
            button.setAttribute('aria-label', label);
            button.setAttribute('title', label);
            if (visual) visual.textContent = language === 'ar' ? 'EN' : 'ع';
        });
        document.querySelectorAll('[data-wishlist-button]').forEach(function (button) {
            var productName = button.dataset.productName || (language === 'ar' ? 'المنتج' : 'product');
            var active = button.classList.contains('active');
            button.setAttribute('aria-pressed', active ? 'true' : 'false');
            button.setAttribute('aria-label', language === 'ar'
                ? (active ? 'إزالة ' + productName + ' من المفضلة' : 'إضافة ' + productName + ' إلى المفضلة')
                : (active ? 'Remove ' + productName + ' from favorites' : 'Add ' + productName + ' to favorites'));
        });
    }

    function localizeSubtree(container, language) {
        var walker = document.createTreeWalker(container, NodeFilter.SHOW_TEXT, {
            acceptNode: function (node) {
                var parent = node.parentElement;
                if (!parent || parent.closest('script, style, noscript, code, pre, [contenteditable="true"], [data-no-translate]')) return NodeFilter.FILTER_REJECT;
                return NodeFilter.FILTER_ACCEPT;
            }
        });
        var textNodes = [];
        while (walker.nextNode()) textNodes.push(walker.currentNode);
        textNodes.forEach(function (node) {
            var translated = localizeValue(node.nodeValue, language);
            if (translated !== node.nodeValue) node.nodeValue = translated;
        });

        var attributeElements = [];
        if (container.nodeType === Node.ELEMENT_NODE && container.matches('[placeholder], [aria-label], [title]')) attributeElements.push(container);
        if (container.querySelectorAll) attributeElements = attributeElements.concat(Array.prototype.slice.call(container.querySelectorAll('[placeholder], [aria-label], [title]')));
        attributeElements.forEach(function (element) {
            ['placeholder', 'aria-label', 'title'].forEach(function (attribute) {
                if (!element.hasAttribute(attribute)) return;
                var value = element.getAttribute(attribute);
                var translated = localizeValue(value, language);
                if (translated !== value) element.setAttribute(attribute, translated);
            });
        });
        updateLanguageButtons(language);
    }

    function applyLanguage(language) {
        var root = document.documentElement;
        root.lang = language;
        root.dir = language === 'ar' ? 'rtl' : 'ltr';
        localizeSubtree(document.body, language);
        document.dispatchEvent(new CustomEvent('myshop:languagechange', { detail: { language: language } }));
    }

    var language = safeLanguage();
    try { localStorage.setItem(key, language); } catch (_) { }
    applyLanguage(language);

    document.addEventListener('click', function (event) {
        var button = event.target.closest('[data-language-toggle]');
        if (!button) return;
        language = language === 'ar' ? 'en' : 'ar';
        try { localStorage.setItem(key, language); } catch (_) { }
        applyLanguage(language);
    });

    if ('MutationObserver' in window) {
        var observer = new MutationObserver(function (mutations) {
            mutations.forEach(function (mutation) {
                if (mutation.type === 'characterData') {
                    var translated = localizeValue(mutation.target.nodeValue, language);
                    if (translated !== mutation.target.nodeValue) mutation.target.nodeValue = translated;
                    return;
                }
                mutation.addedNodes.forEach(function (node) {
                    if (node.nodeType === Node.TEXT_NODE) localizeSubtree(node.parentElement, language);
                    else if (node.nodeType === Node.ELEMENT_NODE) localizeSubtree(node, language);
                });
            });
        });
        observer.observe(document.body, { childList: true, characterData: true, subtree: true });
    }
})();
