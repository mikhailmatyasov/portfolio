import { enableProdMode } from '@angular/core';
import { platformBrowserDynamic } from '@angular/platform-browser-dynamic';

import { AppModule } from './app/app.module';
import { environment } from './environments/environment';

if (environment.production) {
    enableProdMode();
}

platformBrowserDynamic().bootstrapModule(AppModule)
                        .catch(err => {
                            console.error(err);

                            let elem = document.getElementById('id-ef68');

                            if (elem) {
                                elem.parentNode.removeChild(elem);
                            }

                            elem = document.createElement('div');
                            elem.innerText = 'Error occurred. Please contact the administrator.';

                            document.body.append(elem);
                        });
