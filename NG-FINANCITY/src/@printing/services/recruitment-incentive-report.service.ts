import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { State } from '@progress/kendo-data-query';
import { BaseReportApiService } from 'src/@common/Infrastructure/base-report-api.service';

@Injectable()
export class RecruitmentIncentiveReportService extends BaseReportApiService {

    constructor(http: HttpClient) {

        super(http, `RecruitmentIncentiveReport`);
    }

    pdf(state: State = null) {

        return this.get('Pdf', state);
    }
}