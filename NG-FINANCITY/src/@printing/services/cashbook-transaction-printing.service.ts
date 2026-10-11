import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BaseReportApiService } from 'src/@common/Infrastructure/base-report-api.service';

@Injectable()
export class CashbookTransactionPrintngService extends BaseReportApiService {

    constructor(http: HttpClient) {

        super(http, `CashbookTransaction`);
    }

    pdf(cbsk: string, spsk: string) {
        return this.get(`Pdf/${cbsk}/${spsk}`, {});
    }
}