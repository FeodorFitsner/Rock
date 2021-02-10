System.register(["../../Templates/PaneledBlockTemplate.js", "../../Controls/DefinedTypePicker.js", "../../Controls/DefinedValuePicker.js", "../../Controls/CampusPicker.js", "vue", "../../Store/Index.js", "../../Elements/TextBox.js", "../../Elements/EmailBox.js", "../../Elements/CurrencyBox.js", "../../Elements/PanelWidget.js", "../../Elements/DatePicker.js"], function (exports_1, context_1) {
    "use strict";
    var PaneledBlockTemplate_js_1, DefinedTypePicker_js_1, DefinedValuePicker_js_1, CampusPicker_js_1, vue_1, Index_js_1, TextBox_js_1, EmailBox_js_1, CurrencyBox_js_1, PanelWidget_js_1, DatePicker_js_1, GalleryAndResult;
    var __moduleName = context_1 && context_1.id;
    return {
        setters: [
            function (PaneledBlockTemplate_js_1_1) {
                PaneledBlockTemplate_js_1 = PaneledBlockTemplate_js_1_1;
            },
            function (DefinedTypePicker_js_1_1) {
                DefinedTypePicker_js_1 = DefinedTypePicker_js_1_1;
            },
            function (DefinedValuePicker_js_1_1) {
                DefinedValuePicker_js_1 = DefinedValuePicker_js_1_1;
            },
            function (CampusPicker_js_1_1) {
                CampusPicker_js_1 = CampusPicker_js_1_1;
            },
            function (vue_1_1) {
                vue_1 = vue_1_1;
            },
            function (Index_js_1_1) {
                Index_js_1 = Index_js_1_1;
            },
            function (TextBox_js_1_1) {
                TextBox_js_1 = TextBox_js_1_1;
            },
            function (EmailBox_js_1_1) {
                EmailBox_js_1 = EmailBox_js_1_1;
            },
            function (CurrencyBox_js_1_1) {
                CurrencyBox_js_1 = CurrencyBox_js_1_1;
            },
            function (PanelWidget_js_1_1) {
                PanelWidget_js_1 = PanelWidget_js_1_1;
            },
            function (DatePicker_js_1_1) {
                DatePicker_js_1 = DatePicker_js_1_1;
            }
        ],
        execute: function () {
            GalleryAndResult = vue_1.defineComponent({
                name: 'GalleryAndResult',
                components: {
                    PanelWidget: PanelWidget_js_1.default
                },
                template: "\n<PanelWidget>\n    <template #header><slot name=\"header\" /></template>\n    <div class=\"row\">\n        <div class=\"col-md-6\">\n            <slot name=\"gallery\" />\n        </div>\n        <div class=\"col-md-6\">\n            <slot name=\"result\" />\n        </div>\n    </div>\n</PanelWidget>"
            });
            exports_1("default", vue_1.defineComponent({
                name: 'Example.ControlGallery',
                components: {
                    PaneledBlockTemplate: PaneledBlockTemplate_js_1.default,
                    DefinedTypePicker: DefinedTypePicker_js_1.default,
                    DefinedValuePicker: DefinedValuePicker_js_1.default,
                    CampusPicker: CampusPicker_js_1.default,
                    GalleryAndResult: GalleryAndResult,
                    TextBox: TextBox_js_1.default,
                    CurrencyBox: CurrencyBox_js_1.default,
                    EmailBox: EmailBox_js_1.default,
                    DatePicker: DatePicker_js_1.default
                },
                data: function () {
                    return {
                        definedTypeGuid: '',
                        definedValueGuid: '',
                        campusGuid: '',
                        definedValue: null,
                        text: 'Some two-way bound text',
                        currency: 1.234,
                        email: 'joe@joes.co',
                        date: null
                    };
                },
                methods: {
                    onDefinedValueChange: function (definedValue) {
                        this.definedValue = definedValue;
                    }
                },
                computed: {
                    campus: function () {
                        return Index_js_1.default.getters['campuses/getByGuid'](this.campusGuid) || null;
                    },
                    campusName: function () {
                        return this.campus ? this.campus.Name : '';
                    },
                    campusId: function () {
                        return this.campus ? this.campus.Id : null;
                    },
                    definedTypeName: function () {
                        var definedType = Index_js_1.default.getters['definedTypes/getByGuid'](this.definedTypeGuid);
                        return definedType ? definedType.Name : '';
                    },
                    definedValueName: function () {
                        return this.definedValue ? this.definedValue.Value : '';
                    }
                },
                template: "\n<PaneledBlockTemplate>\n    <template v-slot:title>\n        <i class=\"fa fa-flask\"></i>\n        Obsidian Control Gallery\n    </template>\n    <template v-slot:default>\n        <GalleryAndResult>\n            <template #header>\n                TextBox\n            </template>\n            <template #gallery>\n                <TextBox label=\"Text 1\" v-model=\"text\" :maxLength=\"10\" showCountDown />\n                <TextBox label=\"Text 2\" v-model=\"text\" />\n            </template>\n            <template #result>\n                {{text}}\n            </template>\n        </GalleryAndResult>\n        <GalleryAndResult>\n            <template #header>\n                DatePicker\n            </template>\n            <template #gallery>\n                <DatePicker label=\"Date 1\" v-model=\"date\" />\n                <DatePicker label=\"Date 2\" v-model=\"date\" />\n            </template>\n            <template #result>\n                {{date}}\n            </template>\n        </GalleryAndResult>\n        <GalleryAndResult>\n            <template #header>\n                CurrencyBox\n            </template>\n            <template #gallery>\n                <CurrencyBox label=\"Currency 1\" v-model=\"currency\" />\n                <CurrencyBox label=\"Currency 2\" v-model=\"currency\" />\n            </template>\n            <template #result>\n                {{currency}}\n            </template>\n        </GalleryAndResult>\n        <GalleryAndResult>\n            <template #header>\n                EmailBox\n            </template>\n            <template #gallery>\n                <EmailBox label=\"EmailBox 1\" v-model=\"email\" />\n                <EmailBox label=\"EmailBox 2\" v-model=\"email\" />\n            </template>\n            <template #result>\n                {{email}}\n            </template>\n        </GalleryAndResult>\n        <GalleryAndResult>\n            <template #header>\n                Defined Type and Value\n            </template>\n            <template #gallery>\n                <DefinedTypePicker v-model=\"definedTypeGuid\" />\n                <DefinedValuePicker v-model=\"definedValueGuid\" @update:model=\"onDefinedValueChange\" :definedTypeGuid=\"definedTypeGuid\" />\n            </template>\n            <template #result>\n                <p>\n                    <strong>Defined Type Guid</strong>\n                    {{definedTypeGuid}}\n                    <span v-if=\"definedTypeName\">({{definedTypeName}})</span>\n                </p>\n                <p>\n                    <strong>Defined Value Guid</strong>\n                    {{definedValueGuid}}\n                    <span v-if=\"definedValueName\">({{definedValueName}})</span>\n                </p>\n            </template>\n        </GalleryAndResult>\n        <GalleryAndResult>\n            <template #header>\n                CampusPicker\n            </template>\n            <template #gallery>\n                <CampusPicker v-model=\"campusGuid\" />\n                <CampusPicker v-model=\"campusGuid\" label=\"Campus 2\" />\n            </template>\n            <template #result>\n                <p>\n                    <strong>Campus Guid</strong>\n                    {{campusGuid}}\n                    <span v-if=\"campusName\">({{campusName}})</span>\n                </p>\n                <p>\n                    <strong>Campus Id</strong>\n                    {{campusId}}\n                </p>\n            </template>\n        </GalleryAndResult>\n    </template>\n</PaneledBlockTemplate>"
            }));
        }
    };
});
//# sourceMappingURL=ControlGallery.js.map