import { Component, OnInit } from '@angular/core';
import { AppMenuService } from 'src/app/services/app-menu-service';
import { INavigateMenuItem, logoPicture } from 'src/@common/utils';
import { Router } from '@angular/router';

@Component({
  selector: 'fc-home',
  templateUrl: './home.page.html',
  styles: [`

   ion-button div {
      width : 100% !important;
      display: flex;
      flex-flow: column nowrap;
      align-items: center;
    }
  `]
})
export class HomePage implements OnInit {

  source: Array<INavigateMenuItem[]> = new Array();

  constructor(private _appMenuService: AppMenuService,
    private _router: Router) {
  }

  ngOnInit() {

    //to change one dimension array [] to two dimensional array like this [[]].
    //assign %2 to divided into two dimension array.
    this.source = this._appMenuService.appPages.filter(x => x.title != "Home")
      .reduce((rows, key, index) => (index % 2 == 0 ? rows.push([key])
        : rows[rows.length - 1].push(key)) && rows, []);
  }

  handleNav(nav: INavigateMenuItem) {

    this._router.navigate([nav.path]);
  }

  getPicture(key: string) {

    return logoPicture[key];
  }
}
