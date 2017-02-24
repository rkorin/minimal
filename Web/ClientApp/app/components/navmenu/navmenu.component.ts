import { Component } from '@angular/core';

import {AuthService} from '../../common';

@Component({
    selector: 'nav-menu',
    template: require('./navmenu.component.html'),
    styles: [require('./navmenu.component.css')]
})
export class NavMenuComponent {

    constructor(protected auth: AuthService) {
    }

    public role(name: string): boolean { 
        if (this.auth && this.auth.Roles)
            return this.auth.Roles[name];
        return false;
    }
    public claim(name: string): boolean { 
        if (this.auth && this.auth.Claims)
            if (this.auth.Claims[name])
                return true;
        return false;
    }
}
