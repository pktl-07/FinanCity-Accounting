import { Injectable } from '@angular/core';
import { BaseApiService } from 'src/@common/Infrastructure/base.api.service';
import { HttpClient } from '@angular/common/http';

@Injectable()
export class AccountCategoryService extends BaseApiService {

    constructor(http: HttpClient) {

        super(http, `AccountCategory`);
    }
}