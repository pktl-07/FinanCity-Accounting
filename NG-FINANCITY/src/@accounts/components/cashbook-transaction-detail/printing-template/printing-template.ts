import { Component, OnDestroy, OnInit, ViewChild } from "@angular/core";
import { FormBuilder, FormControl, Validators } from "@angular/forms";
import { of } from "rxjs";
import { CashbookTransactionService } from "src/@accounts/services/cashbook-transaction.service";
import { FilterItemService } from "src/@common/components/p-filter-panel/p-filter-item-service";
import { FilterComponent } from "src/@common/components/p-filter-panel/p-filter-panel-component";
import { PModalFormComponent } from "src/@common/components/p-modal-form/p-modal-form.component";
import { PToolbarComponent } from "src/@common/components/p-toolbar/p-toolbar.component";
import { DetailFormBase } from "src/@common/Infrastructure/detail.form.base";
import { makeFormTouched, SubSink } from "src/@common/service/utility-service";
import { CashbookTransactionPrintngService } from "src/@printing/services/cashbook-transaction-printing.service";
import { SalespersonManagementService } from "src/@sales/services/salesperson-management.service";

@Component({
    selector: `'app-cashbook-printing-template`,
    templateUrl: './printing-template.html',
    providers: [
        SalespersonManagementService,
        FilterItemService,
        CashbookTransactionPrintngService
    ]
})
export class PrintingTemplate extends DetailFormBase implements OnInit, OnDestroy {

    @ViewChild('filter', { static: true }) filter: FilterComponent;
    @ViewChild(PToolbarComponent, { static: true }) private _toolbar: PToolbarComponent;
    @ViewChild(PModalFormComponent, { static: true }) public _modalForm: PModalFormComponent;
    CBSK: string;

    private _subsink = new SubSink();

    constructor(fb: FormBuilder,
        public _salespersonManagementService: SalespersonManagementService,
        public _service: CashbookTransactionPrintngService) {

        super(fb);
    }

    ngOnDestroy(): void {
        this._subsink.unsubscribe()
    }

    ngOnInit(): void {
        this._modalForm
            .controls
            .Ready(this.formReady)
            .Success(this.OnSuccess)
            .ExportToPdf({
                action: () => {
                    makeFormTouched(this.form)
                    if (this.form.valid) {
                        return this._service.pdf(this.CBSK, this.form.get('SPSK').value)
                    }
                    return of('')
                }
            })

        this.form = this.fb.group(
            {
                SPSK: new FormControl(null, [Validators.required]),
                NRIC: new FormControl()
            }
        );

        this._salespersonManagementService.asyncFinds();

        this._subsink.sink = this.form.get('SPSK').valueChanges
            .subscribe(value => {
                if (value != null) {
                    this._subsink.sink = this._salespersonManagementService.findByKey(value)
                        .subscribe((data: any) => {
                            this.form.get('NRIC').patchValue(data?.Person?.NRIC)
                        })
                }
            });
    }
}