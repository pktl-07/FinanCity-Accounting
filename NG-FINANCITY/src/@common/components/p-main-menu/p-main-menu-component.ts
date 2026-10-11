import { Component } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'p-main-menu',
  templateUrl: './p-main-menu-component.html',
  styles: [
    `
      .active{
        font-weight:bold;
      }

      .dropdown-toggle::after {
        display:none;
      }

     .item-width{
        min-width:120px;
      }
    `
  ]
})
export class PMainMenuComponent {

  public items: any;
  public currentPage: any;

  constructor(private _router: Router) {

    this.items = this.mapItems(_router.config);
  }

  onSelect(item: any): void {

    if (!item.items) {
      this.currentPage = item;
      this._router.navigateByUrl(item.path);
    }
  }

  isActivePage(item: any): boolean {

    if (this.currentPage == undefined)
      return false;

    return item.path == this.currentPage.path;
  }

  // convert the routes to menu items.
  private mapItems(routes: any[], path?: string, depth: number = 0): any[] {

    return routes.filter(x => (x.display && x.display) != 'none')
      .map(item => {

        const result: any = {
          text: item.text,
          path: (path ? `${path}/` : '') + (item.outlet ? `(${item.outlet}:${item.path})` : item.path),
          display: item.display ? item.display : 'display',
          icon: item.icon ? item.icon : '',
          depth: depth,
        };

        if (item.children)
          result.items = this.mapItems(item.children, result.path, depth + 1);

        return result;
      });
  }

  public onLogoutClick() {

    localStorage.removeItem('userToken');
    this._router.navigateByUrl("/Login");
  }
}

