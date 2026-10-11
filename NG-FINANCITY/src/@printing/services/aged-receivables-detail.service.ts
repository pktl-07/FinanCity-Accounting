import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { State } from '@progress/kendo-data-query';
import { BaseReportApiService } from 'src/@common/Infrastructure/base-report-api.service';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

@Injectable()
export class AgedReceivableDetailReportSerivce extends BaseReportApiService {

    private _unsubscribe = new Subject();
    constructor(http: HttpClient) {

        super(http, `AgedReceivableDetail`);
    }
   
    pdf(state: State = null) {

        return this.get('Pdf', state).pipe(takeUntil(this._unsubscribe));
    }
}