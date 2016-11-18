"use strict";
var handler = require('./handler');
var express = require('express');
var router = express.Router();
var config, def, messages, data;
var jwt = require('jsonwebtoken');
var crypto = require('crypto');

router.init = function (app) {
    config = app.get('config');
    def = app.get('def');
    messages = app.get('messages');
    data = { action: 'init', conn: config.connString.replace('@dbName', config.dbName) }
    handler.init(app, data);
}

router.post('/api/validate/token', function (req, res, next) {
    try {
        var ret = false;
        var token = req.body.token || req.query.token || req.headers['x-access-token'];
        if (token) {
            jwt.verify(token, config.jwtKey, function (error, decoded) {
                if (!error) {
                    ret = true;
                }
                res.status(200).send(ret);
            });
        } else {
            let err = new def.NError(400, messages.errTokenNotFound, messages.messTokenNotFound);
            next(err);
        }
    } catch (error) {
        let err = new def.NError(500, messages.errInternalServerError, error.message);
        next(err);
    }
});

//authenticate
router.post('/api/authenticate', function (req, res, next) {
    try {
        let auth = req.body.auth;
        let err;
        if (auth) {
            var data = { action: 'authenticate', auth: auth };
            handler.edgePush(res, next, 'authenticate', data);
        }
        else {
            let err = new def.NError(404, messages.errAuthStringNotFound, messages.messAuthStringinPostRequest);
            next(err);
        }
    } catch (error) {
        let err = new def.NError(500, messages.errInternalServerError, error.message);
        next(err);
    }
});

//forgot password url
router.post('/api/send/password', function (req, res, next) {
    try {
        let auth = req.body.auth;
        if (auth) {
            jwt.verify(auth, config.jwtKey, function (error, decoded) {
                if (error) {
                    res.status(406).send(false);
                } else {

                    //let random = crypto.randomBytes(4).toString('hex');
                    let url = `<a href='${config.host}'>${config.host}</a>`;
                    let htmlBody = config.sendPassword.mailBody.replace('@url', url);
                    let emailItem = config.sendMail;
                    emailItem.htmlBody = htmlBody;
                    emailItem.subject = config.forgotPassword.subject;
                    emailItem.to = decoded.data;
                    var data = { action: 'new:password', data: emailItem, };
                    handler.edgePush(res, next, 'common:result', data);

                    // let random = crypto.randomBytes(4).toString('hex');
                    // let url = `<a href='${config.host}'>${config.host}</a>`;
                    // let htmlBody = config.sendPassword.mailBody.replace('@pwd', random).replace('@url', url);
                    // let emailItem = config.sendMail;
                    // emailItem.htmlBody = htmlBody;
                    // emailItem.subject = config.forgotPassword.subject;
                    // emailItem.to = decoded.data;
                    // handler.sendMail(res, next, emailItem);
                }
            });
        } else {
            res.status(404).send(false);
        }
    } catch (error) {
        let err = new def.NError(500, messages.errInternalServerError, error.message);
        next(err);
    }
});

//forgot password
router.post('/api/forgot/password', function (req, res, next) {
    try {
        let auth = req.body.auth;
        if (auth) {
            let email = Buffer.from(auth, 'base64').toString();
            var data = { action: 'isEmailExist', email: email };
            //verify email if it exists and then send url to the verified mail
            handler.edgePush(res, next, 'forgot:passowrd', data);
        } else {
            let err = new def.NError(404, messages.errAuthStringNotFound, messages.messAuthStringinPostRequest);
            next(err);
        }
    } catch (error) {
        let err = new def.NError(500, messages.errInternalServerError, error.message);
        next(err);
    }
});
router.post('/api/create/account', function (req, res, next) {
    try {
        let account = req.body;
        if (account) {
            let data = { action: 'create:account', account: account };
            handler.edgePush(res, next, 'common:result', data);
        } else {
            let err = new def.NError(404, messages.errAuthStringNotFound, messages.messAuthStringinPostRequest);
            next(err);
        }
    } catch (error) {
        let err = new def.NError(500, messages.errInternalServerError, error.message);
        next(err);
    }
});

router.all('/api*', function (req, res, next) {
    // implementation for token verification
    try {
        var token = req.body.token || req.query.token || req.headers['x-access-token'];
        if (token) {
            jwt.verify(token, config.jwtKey, function (error, decoded) {
                if (error) {
                    let err = new def.NError(401, messages.errInvalidToken, messages.messInvalidToken);
                    next(err);
                }
                else {
                    req.user = decoded;
                    next();
                }
            });
        }
        else {
            let err = new def.NError(400, messages.errTokenNotFound, messages.messTokenNotFound);
            next(err);
        }
    } catch (error) {
        let err = new def.NError(500, messages.errInternalServerError, error.message);
        next(err);
    }
});

//change password. auth is base64(email:hashOfOldPassword:hashOfNewPassword)
router.post('/api/change/password', function (req, res, next) {
    try {
        let auth = req.body.auth;
        if (auth) {
            let emailItem = config.sendMail;
            emailItem.htmlBody = config.changePassword.mailBody;
            emailItem.subject = config.changePassword.subject;
            var data = {
                action: 'change:password', auth: auth
                , emailItem: emailItem
            };
            handler.edgePush(res, next, 'common:result', data);
        } else {
            let err = new def.NError(404, messages.errAuthStringNotFound, messages.messAuthStringinPostRequest);
            next(err);
        }
    } catch (error) {
        let err = new def.NError(500, messages.errInternalServerError, error.message);
        next(err);
    }
});
router.get('/api/current/offer', function (req, res, next) {
    try {
        let data = { action: 'sql:query', sqlKey: 'GetCurrentOffer', sqlParms: {} };
        handler.edgePush(res, next, 'common:result:data', data);
    } catch (error) {
        let err = new def.NError(500, messages.errInternalServerError, error.message);
        next(err);
    }
});

router.post('/api/order', function (req, res, next) {
    try {
        let data = { action: 'save:order', order: req.body.order, email: req.user.email };
        handler.edgePush(res, next, 'common:result:no:data', data);
    } catch (error) {
        let err = new def.NError(500, messages.errInternalServerError, error.message);
        next(err);
    }
});

router.get('/api/profile', function (req, res, next) {
    try {
        let data = { action: 'sql:query', sqlKey: 'GetProfile', sqlParms: { email: req.user.email } };
        handler.edgePush(res, next, 'common:result:data', data);
    } catch (error) {
        let err = new def.NError(500, messages.errInternalServerError, error.message);
        next(err);
    }
});

router.post('/api/profile', function (req, res, next) {
    try {
        let data = {
            action: 'update:insert:profile',
            profile: req.body.profile,
            email: req.user.email,
            isUpdate: req.body.profile.id ? true : false
        };
        handler.edgePush(res, next, 'common:result:no:data', data);
    } catch (error) {
        let err = new def.NError(500, messages.errInternalServerError, error.message);
        next(err);
    }
});

router.get('/api/order/headers', function (req, res, next) {
    try {
        let data = { action: 'sql:query', sqlKey: 'GetOrderHeaders', sqlParms: { email: req.user.email } };
        handler.edgePush(res, next, 'common:result:data', data);
    } catch (error) {
        let err = new def.NError(500, messages.errInternalServerError, error.message);
        next(err);
    }
});

router.get('/api/shipping/address', function (req, res, next) {
    try {
        let data = { action: 'sql:query', sqlKey: 'GetShippingAddress', sqlParms: { email: req.user.email } };
        handler.edgePush(res, next, 'common:result:data', data);
    } catch (error) {
        let err = new def.NError(500, messages.errInternalServerError, error.message);
        next(err);
    }
});

router.post('/api/shipping/address', function (req, res, next) {
    try {
        let data = {
            action: 'update:insert:address',
            addresses: req.body.addresses,
            email: req.user.email
        };
        handler.edgePush(res, next, 'common:result:no:data', data);
    } catch (error) {
        let err = new def.NError(500, messages.errInternalServerError, error.message);
        next(err);
    }
});

router.get('/api/credit/card', function (req, res, next) {
    try {
        let data = { action: 'sql:query', sqlKey: 'GetCreditCards', sqlParms: { email: req.user.email } };
        handler.edgePush(res, next, 'common:result:data', data);
    } catch (error) {
        let err = new def.NError(500, messages.errInternalServerError, error.message);
        next(err);
    }
});

router.delete('/api/credit/card', function (req, res, next) {
    try {
        let data = {
            action: 'sql:non:query',
            sqlKey: 'DeleteCreditCard',
            sqlParms: {
                id: req.body.id,
                email: req.user.email
            }
        };
        handler.edgePush(res, next, 'common:result:no:data', data);
    } catch (error) {
        let err = new def.NError(500, messages.errInternalServerError, error.message);
        next(err);
    }
});

router.post('/api/credit/card', function (req, res, next) {
    try {
        let data = {
            action: 'insert:credit:card',
            card: req.body.card,
            email: req.user.email
        };
        handler.edgePush(res, next, 'common:result:no:data', data);
    } catch (error) {
        let err = new def.NError(500, messages.errInternalServerError, error.message);
        next(err);
    }
});

router.get('/api/shipping/address/default', function (req, res, next) {
    try {
        let data = { action: 'sql:query', sqlKey: 'GetDefaultShippingAddress', sqlParms: { userId: req.user.userId } };
        handler.edgePush(res, next, 'common:result:data', data);
    } catch (error) {
        let err = new def.NError(500, messages.errInternalServerError, error.message);
        next(err);
    }
});

router.get('/api/shipping/address', function (req, res, next) {
    try {
        let data = { action: 'sql:query', sqlKey: 'GetAllShippingAddresses', sqlParms: { userId: req.user.userId } };
        handler.edgePush(res, next, 'common:result:data', data);
    } catch (error) {
        let err = new def.NError(500, messages.errInternalServerError, error.message);
        next(err);
    }
});

module.exports = router;
