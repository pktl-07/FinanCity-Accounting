import { Injectable } from '@angular/core';
import { BaseApiService } from 'src/@common/Infrastructure/base.api.service';
import { HttpClient } from '@angular/common/http';

@Injectable({ providedIn: 'root' })
export class TNC_TimeFrameService extends BaseApiService {

    constructor(http: HttpClient) {

        super(http, `TNC_TimeFrame`);
    }
}