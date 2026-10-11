import { Injectable } from '@angular/core';
import { BaseApiService } from 'src/@common/Infrastructure/base.api.service';
import { HttpClient } from '@angular/common/http';

@Injectable()
export class PostalCodeService extends BaseApiService {

    constructor(public http: HttpClient) {

        super(http, `PostalCode`);
    }
}