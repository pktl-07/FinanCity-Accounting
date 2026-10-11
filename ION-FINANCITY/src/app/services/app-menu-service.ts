import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { INavigateMenuItem } from 'src/@common/utils';

@Injectable({ providedIn: 'root' })
export class AppMenuService {

    readonly appPages: Array<INavigateMenuItem> = this.mapItems(this.router.config as Array<INavigateMenuItem>);

    constructor(private router: Router) {
    }

    private mapItems(routes: Array<INavigateMenuItem>, path?: string): any[] {

        return routes.filter(x => (x.display && x.display) != 'none')
            .map(item => {
                const result: any = {
                    title: item.title,
                    path: (path ? `${path}/` : '') + `/${item.path}`,
                    display: item.display ? item.display : 'display',
                    icon: item.icon,
                    color: item.color ? item.color : 'primary',
                };

                if (item.children) {
                    result.children = this.mapItems(item.children, item.path);
                }
                return result;
            });
    }
}