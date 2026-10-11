import { Component, Input, ViewChild, OnInit, OnDestroy } from '@angular/core';
import { CurrencyRateService } from 'src/app/services/currency.rate.service';
import { FilterConnectorService } from 'src/@common/components/validator/fc.filter-connector.service';
import { Router } from '@angular/router';
import { FCFilterToolbarComponent } from 'src/@common/components/toolbar/fc-filter-toolbar.component';
import { BroadcasterService } from 'src/app/services/broadcaster.service';
import { SubSink } from 'src/@common/utils';

@Component({
    selector: 'fc-mortgage-sales-filter',
    template: `
    <fc-filter-toolbar [connector]="connector" addable="true" (add)="createNew()">
        <fc-filter-composite title="Transaction Filter">
            <fc-filter-item field="SalesNo" label="Trans. No." type="text"></fc-filter-item>
            <fc-filter-item field="TransactionDate" label="Transaction Date" type="date"></fc-filter-item>
            <fc-filter-item field="CommissionAmount" label="Commission Amount" type="currency"></fc-filter-item>
            <fc-filter-item field="PropertyAddress" label="NRIC" type="text"></fc-filter-item>
            <fc-filter-item field="ClientName" label="Address" type="text"></fc-filter-item>
            <fc-filter-item field="SalespersonName" label="Salesperson Name" type="text"></fc-filter-item>
            <fc-filter-item field="MortgageBank" label="Mortgage Bank" type="text"></fc-filter-item>
            <fc-filter-item field="CommissionAmount" label="Comm. Amount" type="currency"></fc-filter-item>
            <fc-filter-item field="CurrencyCode" label="Currency" [items]="_crrencyRateService" value="SysKey"
                text="CurrencyCode" type="dropdown"></fc-filter-item>
        <fc-filter-item field="RecordStatus" label="Status" [items]="recordStatus" value="value" text="text"
        type="dropdown">
      </fc-filter-item>
    </fc-filter-composite>
  </fc-filter-toolbar>
  `,
    providers: [
        CurrencyRateService]
})
export class MortgageSalesFilterPage implements OnInit, OnDestroy {

    @Input() connector: FilterConnectorService;
    @ViewChild(FCFilterToolbarComponent, { static: true }) _filterTool: FCFilterToolbarComponent;

    private _subsink = new SubSink();

    readonly recordStatus = [
        { value: -3, text: 'Save Draft' },
        { value: -2, text: 'Reject' },
        { value: -1, text: 'Submit' },
        { value: 2, text: 'Enter Save' },
        { value: 3, text: 'Enter Update' },
        { value: 6, text: 'Void' },
        { value: 8, text: 'Post' }];

    constructor(public _crrencyRateService: CurrencyRateService,
        private _router: Router,
        private _broadcaster: BroadcasterService) {

        _crrencyRateService.asyncFinds();
    }

    ngOnDestroy(): void {
        this._subsink.unsubscribe();
    }

    ngOnInit(): void {
        
        this._subsink.sink = this._broadcaster.on('property-transaction-reload')
            .subscribe(() => {
                this._filterTool.refresh();
            });
    }

    async createNew() {
        return await this._router.navigateByUrl(`/property-transaction-detail`);
    }
}
