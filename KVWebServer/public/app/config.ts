export var urlHash = {
    'post:authenticate': '/api/authenticate',
    'post:validate:token': '/api/validate/token',
    'post:forgot:password': '/api/forgot/password',
    'post:send:password': '/api/send/password',
    'post:change:password': '/api/change/password',
    'post:create:account': '/api/create/account',
    'get:current:offer': '/api/current/offer',
    'post:save:order': '/api/order',
    'get:user:profile': '/api/profile',
    'post:save:profile': '/api/profile',
    'get:order:headers': '/api/order/headers',
    'get:shipping:address': '/api/shipping/address',
    'post:shipping:address': '/api/shipping/address',
    'get:credit:card': '/api/credit/card',
    'delete:credit:card': '/api/credit/card',
    'insert:credit:card': '/api/credit/card',
    'get:default:shipping:address':'/api/shipping/address/default',
    'get:all:shipping:addresses':'/api/shipping/address'
};
export var messages = {
    'mess:order:intro:text': 'Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed a tortor imperdiet, porttitor purus ut, pretium diam. Nulla convallis vel ipsum quis mattis. Nam in erat massa. Praesent bibendum dapibus lectus, nec imperdiet eros pharetra et. Vestibulum euismod velit sit amet nulla ornare iaculis viverra eget augue. Etiam dolor lacus, rhoncus in rhoncus eget, faucibus at nisi. Nulla nec mollis orci. Pellentesque pharetra facilisis dolor vel tincidunt. Suspendisse ullamcorper fermentum nunc, a dapibus tellus imperdiet vitae.',
    'mess:order:holiday:gift': 'Holiday Gift - Yes I\'m interested, please contact me',
    'mess:order:minimum:request': 'Minimum request 6 bottles',
    'mess:order:bottom:notes': 'Wines in 6 bottle packages are subject to change',
    'mess:approve:heading': 'Please review your shipping address & payment method information for your order.'
};
export var viewBoxConfig = {
    '/login': { home: true, needHelp: false, order: false, myAccount: false, menuBar: false },
    '/order': { home: true, needHelp: true, order: false, myAccount: true, menuBar: false },
    '/approve': { home: true, needHelp: true, order: false, myAccount: true, menuBar: false },
    '/receipt': { home: true, needHelp: true, order: false, myAccount: true, menuBar: false },
    '/profile': { home: true, needHelp: false, order: true, myAccount: false, menuBar: true },
    '/order/history': { home: true, needHelp: false, order: true, myAccount: false, menuBar: true },
    '/shipping/address': { home: true, needHelp: false, order: true, myAccount: false, menuBar: true },
    '/payment/method': { home: true, needHelp: false, order: true, myAccount: false, menuBar: true },
    '/change/password': { home: true, needHelp: false, order: true, myAccount: false, menuBar: true },
    '/create/account': { home: true, needHelp: false, order: false, myAccount: false, menuBar: false },
    '/forgot/password': { home: true, needHelp: false, order: false, myAccount: false, menuBar: false },
    '/send/password': { home: true, needHelp: false, order: false, myAccount: false, menuBar: false },
    '/approve/order': { home: true, needHelp: true, order: false, myAccount: true, menuBar: false }
};