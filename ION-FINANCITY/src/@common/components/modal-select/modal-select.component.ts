
import {
    AfterContentInit,
    Component,
    forwardRef,
    OnDestroy,
    OnInit,
    Output,
    Input,
    Injector,
    Optional,
    Host,
    SkipSelf,
    ContentChild,
    EventEmitter
} from '@angular/core';
import {
    ControlValueAccessor,
    NG_VALUE_ACCESSOR,
    NgControl,
    AbstractControl,
    FormGroup,
    FormBuilder,
    ControlContainer,
    FormControl
} from '@angular/forms';
import { faChevronDoubleDown } from '@fortawesome/pro-solid-svg-icons'
import { SubSink } from 'src/@common/utils';
import { FCModalSelectContentDirective } from './modal-select-content-template';

@Component({
    host: {
        '(click)': 'handleClick($event)',
    },
    selector: 'fc-modal-select',
    template: `
        <ion-item button [disabled]="disabled" detail="false" 
         (blur)="onTouched()">
            <ion-label position="stacked">{{label}}</ion-label>
            <ion-input readonly [formControl]="form?.get('value')"></ion-input>
            <ion-button color="medium" slot="end" fill="clear" class="ion-align-self-end">
              <fa-icon [icon]="arrowDoubleDownIcon"></fa-icon>
            </ion-button>
            <ng-template [ngIf]="contentTemplate?.templateRef"
                [ngTemplateOutlet]="contentTemplate?.templateRef">
            </ng-template>
        </ion-item>`,
    providers: [{
        provide: NG_VALUE_ACCESSOR, useExisting: forwardRef(() => ModalSelectComponent), multi: true
    }]
})
export class ModalSelectComponent implements OnInit, ControlValueAccessor, AfterContentInit, OnDestroy {

    @ContentChild(FCModalSelectContentDirective, { static: true }) contentTemplate: FCModalSelectContentDirective;

    readonly arrowDoubleDownIcon = faChevronDoubleDown;

    onChange = (_: any) => { };
    onTouched = () => { };

    @Input() disabled: boolean = false;
    @Input() label: string = "";
    @Output() blur = new EventEmitter();

    form: FormGroup;
    control: AbstractControl;

    private _subsink = new SubSink();

    constructor(
        private _injector: Injector,
        private _fb: FormBuilder,
        @Optional() @Host() @SkipSelf() private _controlContainer: ControlContainer) { }

    ngOnInit(): void {

        this.form = this._fb.group({
            value: null
        })
    }

    ngAfterContentInit(): void {
        const ngControl: NgControl = this._injector.get(NgControl, null);
        if (this._controlContainer && ngControl) {
            this.control = this._controlContainer.control.get(<string>ngControl.name);
            if (this.control && this.control.validator) {
                this.form.get('value').setValidators([this.control.validator]);
                this._subsink.sink = (this.control as FormControl).statusChanges
                    .subscribe(item => {
                        // this.form.get('value').markAsTouched();
                        this.control.markAsTouched()
                    });
            }
        }
    }

    ngOnDestroy(): void {
        this._subsink.unsubscribe();
    }

    writeValue(obj: any): void {
        this.form.get('value').patchValue(obj)
    }

    registerOnChange(fn: any): void {
        this.onChange = fn;
    }

    registerOnTouched(fn: any): void {
        this.onTouched = fn;
        this.blur.emit();
    }

    setDisabledState?(isDisabled: boolean): void {
        this.disabled = isDisabled;
    }

    handleClick(eve: any) {
        this.form.get('value').markAsTouched();
    }
}