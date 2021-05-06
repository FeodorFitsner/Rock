System.register(["../Services/DateKey", "../Services/Email", "../Services/String", "vee-validate"], function (exports_1, context_1) {
    "use strict";
    var DateKey_1, Email_1, String_1, vee_validate_1, isNumeric;
    var __moduleName = context_1 && context_1.id;
    function ruleStringToArray(rulesString) {
        return rulesString.split('|');
    }
    exports_1("ruleStringToArray", ruleStringToArray);
    function ruleArrayToString(rulesArray) {
        return rulesArray.join('|');
    }
    exports_1("ruleArrayToString", ruleArrayToString);
    return {
        setters: [
            function (DateKey_1_1) {
                DateKey_1 = DateKey_1_1;
            },
            function (Email_1_1) {
                Email_1 = Email_1_1;
            },
            function (String_1_1) {
                String_1 = String_1_1;
            },
            function (vee_validate_1_1) {
                vee_validate_1 = vee_validate_1_1;
            }
        ],
        execute: function () {
            vee_validate_1.defineRule('required', (function (value, _a) {
                var optionsJson = _a[0];
                var options = optionsJson ? JSON.parse(optionsJson) : {};
                if (typeof value === 'string') {
                    var allowEmptyString = !!(options.allowEmptyString);
                    if (!allowEmptyString && String_1.isNullOrWhitespace(value)) {
                        return 'is required';
                    }
                    return true;
                }
                if (typeof value === 'number' && value === 0) {
                    return 'is required';
                }
                if (!value) {
                    return 'is required';
                }
                return true;
            }));
            vee_validate_1.defineRule('email', (function (value) {
                // Field is empty, should pass
                if (String_1.isNullOrWhitespace(value)) {
                    return true;
                }
                // Check if email
                if (!Email_1.isEmail(value)) {
                    return 'must be a valid email';
                }
                return true;
            }));
            vee_validate_1.defineRule('notequal', (function (value, _a) {
                var compare = _a[0];
                if (isNumeric(value) && isNumeric(compare)) {
                    if (Number(value) !== Number(compare)) {
                        return true;
                    }
                }
                else if (value !== compare) {
                    return true;
                }
                return "must not equal " + compare;
            }));
            vee_validate_1.defineRule('equal', (function (value, _a) {
                var compare = _a[0];
                if (isNumeric(value) && isNumeric(compare)) {
                    if (Number(value) === Number(compare)) {
                        return true;
                    }
                }
                else if (value === compare) {
                    return true;
                }
                return "must equal " + compare;
            }));
            vee_validate_1.defineRule('gt', (function (value, _a) {
                var compare = _a[0];
                if (isNumeric(value) && isNumeric(compare)) {
                    if (Number(value) > Number(compare)) {
                        return true;
                    }
                }
                else if (value > compare) {
                    return true;
                }
                return "must be greater than " + compare;
            }));
            vee_validate_1.defineRule('gte', (function (value, _a) {
                var compare = _a[0];
                if (isNumeric(value) && isNumeric(compare)) {
                    if (Number(value) >= Number(compare)) {
                        return true;
                    }
                }
                else if (value >= compare) {
                    return true;
                }
                return "must not be less than " + compare;
            }));
            vee_validate_1.defineRule('lt', (function (value, _a) {
                var compare = _a[0];
                if (isNumeric(value) && isNumeric(compare)) {
                    if (Number(value) < Number(compare)) {
                        return true;
                    }
                }
                else if (value < compare) {
                    return true;
                }
                return "must be less than " + compare;
            }));
            vee_validate_1.defineRule('lte', (function (value, _a) {
                var compare = _a[0];
                if (isNumeric(value) && isNumeric(compare)) {
                    if (Number(value) <= Number(compare)) {
                        return true;
                    }
                }
                else if (value <= compare) {
                    return true;
                }
                return "must not be more than " + compare;
            }));
            vee_validate_1.defineRule('datekey', (function (value) {
                var asString = value;
                if (!DateKey_1.default.getYear(asString)) {
                    return 'must have a year';
                }
                if (!DateKey_1.default.getMonth(asString)) {
                    return 'must have a month';
                }
                if (!DateKey_1.default.getDay(asString)) {
                    return 'must have a day';
                }
                return true;
            }));
            /**
             * Is the value numeric?
             * '0.9' => true
             * 0.9 => true
             * '9a' => false
             * @param value
             */
            isNumeric = function (value) {
                if (typeof value === 'number') {
                    return true;
                }
                if (typeof value === 'string') {
                    var asNumber = Number(value);
                    return !isNaN(asNumber) && !isNaN(parseFloat(value));
                }
                return false;
            };
        }
    };
});
//# sourceMappingURL=Index.js.map