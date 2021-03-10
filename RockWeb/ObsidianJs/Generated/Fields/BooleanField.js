System.register(["vue", "./Index", "../Services/Boolean", "../Elements/DropDownList"], function (exports_1, context_1) {
    "use strict";
    var vue_1, Index_1, Boolean_1, DropDownList_1, fieldTypeGuid;
    var __moduleName = context_1 && context_1.id;
    return {
        setters: [
            function (vue_1_1) {
                vue_1 = vue_1_1;
            },
            function (Index_1_1) {
                Index_1 = Index_1_1;
            },
            function (Boolean_1_1) {
                Boolean_1 = Boolean_1_1;
            },
            function (DropDownList_1_1) {
                DropDownList_1 = DropDownList_1_1;
            }
        ],
        execute: function () {
            fieldTypeGuid = '1EDAFDED-DFE6-4334-B019-6EECBA89E05A';
            exports_1("default", Index_1.registerFieldType(fieldTypeGuid, vue_1.defineComponent({
                name: 'BooleanField',
                components: {
                    DropDownList: DropDownList_1.default
                },
                props: Index_1.getFieldTypeProps(),
                data: function () {
                    var trueVal = Boolean_1.asTrueFalseOrNull(true);
                    var falseVal = Boolean_1.asTrueFalseOrNull(false);
                    var yesVal = Boolean_1.asYesNoOrNull(true);
                    var noVal = Boolean_1.asYesNoOrNull(false);
                    return {
                        internalValue: '',
                        dropDownListOptions: [
                            { key: falseVal, text: noVal, value: falseVal },
                            { key: trueVal, text: yesVal, value: trueVal }
                        ]
                    };
                },
                computed: {
                    valueAsYesNoOrNull: function () {
                        return Boolean_1.asYesNoOrNull(this.modelValue);
                    }
                },
                watch: {
                    internalValue: function () {
                        this.$emit('update:modelValue', this.internalValue);
                    },
                    modelValue: {
                        immediate: true,
                        handler: function () {
                            this.internalValue = Boolean_1.asTrueFalseOrNull(this.modelValue) || '';
                        }
                    }
                },
                template: "\n<DropDownList v-if=\"isEditMode\" v-model=\"internalValue\" :options=\"dropDownListOptions\" />\n<span v-else>{{ valueAsYesNoOrNull }}</span>"
            })));
        }
    };
});
//# sourceMappingURL=BooleanField.js.map