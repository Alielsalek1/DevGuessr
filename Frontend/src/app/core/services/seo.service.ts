import { Injectable, Inject } from '@angular/core';
import { Title, Meta } from '@angular/platform-browser';
import { Router, NavigationEnd, ActivatedRoute } from '@angular/router';
import { filter, map, mergeMap } from 'rxjs/operators';
import { DOCUMENT } from '@angular/common';

@Injectable({
  providedIn: 'root'
})
export class SeoService {
  constructor(
    private titleService: Title,
    private metaService: Meta,
    private router: Router,
    private activatedRoute: ActivatedRoute,
    @Inject(DOCUMENT) private dom: Document
  ) {}

  private updateCanonicalUrl(url: string) {
    const head = this.dom.getElementsByTagName('head')[0];
    let element: HTMLLinkElement | null = this.dom.querySelector(`link[rel='canonical']`) || null;
    if (element === null) {
      element = this.dom.createElement('link') as HTMLLinkElement;
      element.setAttribute('rel', 'canonical');
      head.appendChild(element);
    }
    element.setAttribute('href', url);
  }

  init() {
    this.router.events.pipe(
      filter(event => event instanceof NavigationEnd),
      map(() => this.activatedRoute),
      map(route => {
        while (route.firstChild) {
          route = route.firstChild;
        }
        return route;
      }),
      filter(route => route.outlet === 'primary'),
      mergeMap(route => route.data)
    ).subscribe(data => {
      if (data['title']) {
        this.titleService.setTitle(data['title']);
        this.metaService.updateTag({ property: 'og:title', content: data['title'] });
        this.metaService.updateTag({ name: 'twitter:title', content: data['title'] });
      }
      
      if (data['description']) {
        this.metaService.updateTag({ name: 'description', content: data['description'] });
        this.metaService.updateTag({ property: 'og:description', content: data['description'] });
        this.metaService.updateTag({ name: 'twitter:description', content: data['description'] });
      }

      const url = `https://devguessr.site${this.router.url.split('?')[0].split('#')[0]}`;
      this.metaService.updateTag({ property: 'og:url', content: url });
      this.updateCanonicalUrl(url);
    });
  }
}
