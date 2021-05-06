﻿// <copyright>
// Copyright by the Spark Development Network
//
// Licensed under the Rock Community License (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.rockrms.com/license
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>
//
import PaneledBlockTemplate from '../../Templates/PaneledBlockTemplate';
import DefinedTypePicker from '../../Controls/DefinedTypePicker';
import DefinedValuePicker from '../../Controls/DefinedValuePicker';
import CampusPicker from '../../Controls/CampusPicker';
import { defineComponent, PropType } from 'vue';
import store from '../../Store/Index';
import TextBox from '../../Elements/TextBox';
import EmailBox from '../../Elements/EmailBox';
import DefinedValue from '../../ViewModels/CodeGenerated/DefinedValueViewModel';
import Campus from '../../ViewModels/CodeGenerated/CampusViewModel';
import DefinedType from '../../ViewModels/CodeGenerated/DefinedTypeViewModel';
import CurrencyBox from '../../Elements/CurrencyBox';
import PanelWidget from '../../Elements/PanelWidget';
import DatePicker from '../../Elements/DatePicker';
import { RockDateType } from '../../Util/RockDate';
import BirthdayPicker, { BirthdayPickerModel } from '../../Elements/BirthdayPicker';
import NumberUpDown from '../../Elements/NumberUpDown';
import AddressControl, { getDefaultAddressControlModel } from '../../Controls/AddressControl';
import Toggle from '../../Elements/Toggle';
import ItemsWithPreAndPostHtml, { ItemWithPreAndPostHtml } from '../../Elements/ItemsWithPreAndPostHtml';
import StaticFormControl from '../../Elements/StaticFormControl';
import ProgressTracker, { ProgressTrackerItem } from '../../Elements/ProgressTracker';
import RockForm from '../../Controls/RockForm';
import RockButton from '../../Elements/RockButton';

const GalleryAndResult = defineComponent({
    name: 'GalleryAndResult',
    components: {
        PanelWidget
    },
    props: {
        splitWidth: {
            type: Boolean as PropType<boolean>,
            default: true
        }
    },
    template: `
<PanelWidget>
    <template #header><slot name="header" /></template>
    <div v-if="splitWidth" class="row">
        <div class="col-md-6">
            <slot name="gallery" />
        </div>
        <div class="col-md-6">
            <slot name="result" />
        </div>
    </div>
    <template v-else>
        <div>
            <slot name="gallery" />
        </div>
        <div>
            <slot name="result" />
        </div>
    </template>
</PanelWidget>`
});

export default defineComponent({
    name: 'Example.ControlGallery',
    components: {
        PaneledBlockTemplate,
        DefinedTypePicker,
        DefinedValuePicker,
        CampusPicker,
        GalleryAndResult,
        TextBox,
        CurrencyBox,
        EmailBox,
        DatePicker,
        BirthdayPicker,
        NumberUpDown,
        AddressControl,
        Toggle,
        ItemsWithPreAndPostHtml,
        StaticFormControl,
        ProgressTracker,
        RockForm,
        RockButton
    },
    data() {
        return {
            definedTypeGuid: '',
            definedValueGuid: '',
            campusGuid: '',
            definedValue: null as DefinedValue | null,
            text: 'Some two-way bound text',
            currency: 1.234,
            email: 'joe@joes.co',
            date: null as RockDateType | null,
            numberUpDown: 1,
            address: getDefaultAddressControlModel(),
            toggle: false,
            birthday: {
                Month: 1,
                Day: 1,
                Year: 2020
            } as BirthdayPickerModel,
            prePostHtmlItems: [
                { PreHtml: '<div class="row"><div class="col-sm-6">', PostHtml: '</div>', SlotName: 'item1' },
                { PreHtml: '<div class="col-sm-6">', PostHtml: '</div></div>', SlotName: 'item2' }
            ] as ItemWithPreAndPostHtml[],
            progressTrackerIndex: 0,
            progressTrackerItems: [
                { Key: 'S', Title: 'Start', Subtitle: 'The beginning' },
                { Key: '1', Title: 'Step 1', Subtitle: 'The first step' },
                { Key: '2', Title: 'Step 2', Subtitle: 'The second step' },
                { Key: '3', Title: 'Step 3', Subtitle: 'The third step' },
                { Key: '4', Title: 'Step 4', Subtitle: 'The fourth step' },
                { Key: '5', Title: 'Step 5', Subtitle: 'The fifth step' },
                { Key: '6', Title: 'Step 6', Subtitle: 'The sixth step' },
                { Key: '7', Title: 'Step 7', Subtitle: 'The seventh step' },
                { Key: '8', Title: 'Step 8', Subtitle: 'The eighth step' },
                { Key: 'F', Title: 'Finish', Subtitle: 'The finish' }
            ] as ProgressTrackerItem[],
            ruleTestCurrency: 1,
            ruleTestText: '',
            rules: 'required'
        };
    },
    methods: {
        onDefinedValueChange(definedValue: DefinedValue | null) {
            this.definedValue = definedValue;
        }
    },
    computed: {
        campus(): Campus | null {
            return store.getters['campuses/getByGuid'](this.campusGuid) || null;
        },
        campusName(): string {
            return this.campus?.Name || '';
        },
        campusId(): number | null {
            return this.campus ? this.campus.Id : null;
        },
        definedTypeName(): string {
            const definedType = store.getters['definedTypes/getByGuid'](this.definedTypeGuid) as DefinedType;
            return definedType?.Name || '';
        },
        definedValueName(): string {
            return this.definedValue?.Value || '';
        }
    },
    template: `
<PaneledBlockTemplate>
    <template v-slot:title>
        <i class="fa fa-flask"></i>
        Obsidian Control Gallery
    </template>
    <template v-slot:default>
        <GalleryAndResult>
            <template #header>
                TextBox
            </template>
            <template #gallery>
                <TextBox label="Text 1" v-model="text" :maxLength="10" showCountDown />
                <TextBox label="Text 2" v-model="text" />
            </template>
            <template #result>
                {{text}}
            </template>
        </GalleryAndResult>
        <GalleryAndResult>
            <template #header>
                DatePicker
            </template>
            <template #gallery>
                <DatePicker label="Date 1" v-model="date" />
                <DatePicker label="Date 2" v-model="date" />
            </template>
            <template #result>
                {{date === null ? 'null' : date}}
            </template>
        </GalleryAndResult>
        <GalleryAndResult>
            <template #header>
                CurrencyBox
            </template>
            <template #gallery>
                <CurrencyBox label="Currency 1" v-model="currency" />
                <CurrencyBox label="Currency 2" v-model="currency" />
            </template>
            <template #result>
                {{currency}}
            </template>
        </GalleryAndResult>
        <GalleryAndResult>
            <template #header>
                EmailBox
            </template>
            <template #gallery>
                <EmailBox label="EmailBox 1" v-model="email" />
                <EmailBox label="EmailBox 2" v-model="email" />
            </template>
            <template #result>
                {{email}}
            </template>
        </GalleryAndResult>
        <GalleryAndResult>
            <template #header>
                BirthdayPicker
            </template>
            <template #gallery>
                <BirthdayPicker label="BirthdayPicker 1" v-model="birthday" />
                <BirthdayPicker label="BirthdayPicker 2" v-model="birthday" />
            </template>
            <template #result>
                {{birthday.Month}} / {{birthday.Day}} / {{birthday.Year}}
            </template>
        </GalleryAndResult>
        <GalleryAndResult>
            <template #header>
                Defined Type and Value
            </template>
            <template #gallery>
                <DefinedTypePicker v-model="definedTypeGuid" />
                <DefinedValuePicker v-model="definedValueGuid" @update:model="onDefinedValueChange" :definedTypeGuid="definedTypeGuid" />
            </template>
            <template #result>
                <p>
                    <strong>Defined Type Guid</strong>
                    {{definedTypeGuid}}
                    <span v-if="definedTypeName">({{definedTypeName}})</span>
                </p>
                <p>
                    <strong>Defined Value Guid</strong>
                    {{definedValueGuid}}
                    <span v-if="definedValueName">({{definedValueName}})</span>
                </p>
            </template>
        </GalleryAndResult>
        <GalleryAndResult>
            <template #header>
                CampusPicker
            </template>
            <template #gallery>
                <CampusPicker v-model="campusGuid" />
                <CampusPicker v-model="campusGuid" label="Campus 2" />
            </template>
            <template #result>
                <p>
                    <strong>Campus Guid</strong>
                    {{campusGuid}}
                    <span v-if="campusName">({{campusName}})</span>
                </p>
                <p>
                    <strong>Campus Id</strong>
                    {{campusId}}
                </p>
            </template>
        </GalleryAndResult>
        <GalleryAndResult>
            <template #header>
                NumberUpDown
            </template>
            <template #gallery>
                <NumberUpDown label="NumberUpDown 1" v-model="numberUpDown" />
                <NumberUpDown label="NumberUpDown 2" v-model="numberUpDown" />
            </template>
            <template #result>
                {{numberUpDown}}
            </template>
        </GalleryAndResult>
        <GalleryAndResult>
            <template #header>
                StaticFormControl
            </template>
            <template #gallery>
                <StaticFormControl label="StaticFormControl 1" v-model="numberUpDown" />
                <StaticFormControl label="StaticFormControl 2" v-model="numberUpDown" />
            </template>
            <template #result>
                {{numberUpDown}}
            </template>
        </GalleryAndResult>
        <GalleryAndResult>
            <template #header>
                AddressControl
            </template>
            <template #gallery>
                <AddressControl v-model="address" />
                <AddressControl label="Address 2" v-model="address" />
            </template>
            <template #result>
                <pre>{{JSON.stringify(address, null, 2)}}</pre>
            </template>
        </GalleryAndResult>
        <GalleryAndResult>
            <template #header>
                Toggle
            </template>
            <template #gallery>
                <Toggle label="Toggle 1" v-model="toggle" />
                <Toggle label="Toggle 2" v-model="toggle" />
            </template>
            <template #result>
                {{toggle}}
            </template>
        </GalleryAndResult>
        <GalleryAndResult>
            <template #header>
                ItemsWithPreAndPostHtml
            </template>
            <template #gallery>
                <TextBox label="Item 1 - Pre Html" v-model="prePostHtmlItems[0].PreHtml" />
                <TextBox label="Item 1 - Post Html" v-model="prePostHtmlItems[0].PostHtml" />
                <TextBox label="Item 2 - Pre Html" v-model="prePostHtmlItems[1].PreHtml" />
                <TextBox label="Item 2 - Post Html" v-model="prePostHtmlItems[1].PostHtml" />
            </template>
            <template #result>
                <ItemsWithPreAndPostHtml :items="prePostHtmlItems">
                    <template #item1>
                        <div style="background-color: #fcc; padding: 5px;">This is item 1</div>
                    </template>
                    <template #item2>
                        <div style="background-color: #ccf; padding: 5px;">This is item 2</div>
                    </template>
                </ItemsWithPreAndPostHtml>
            </template>
        </GalleryAndResult>
        <GalleryAndResult :splitWidth="false">
            <template #header>
                ProgressTracker
            </template>
            <template #gallery>
                <NumberUpDown label="Index" v-model="progressTrackerIndex" :min="-100" :max="100" />
            </template>
            <template #result>
                <ProgressTracker :items="progressTrackerItems" :currentIndex="progressTrackerIndex" />
            </template>
        </GalleryAndResult>
        <GalleryAndResult :splitWidth="false">
            <template #header>
                Rules
            </template>
            <template #gallery>
                <TextBox label="Rules" v-model="rules" help="Try 'required', 'gte:1', 'lt:2', and others. Combine rules like this: 'required|lt:7|gt:6'" />
                <hr />
                <RockForm>
                    <TextBox label="Text" v-model="ruleTestText" :rules="rules" />
                    <CurrencyBox label="Currency" v-model="ruleTestCurrency" :rules="rules" />
                    <RockButton btnType="primary" type="submit">Test</RockButton>
                </RockForm>
            </template>
        </GalleryAndResult>
    </template>
</PaneledBlockTemplate>`
});