import { Routes } from '@angular/router';

export const routes: Routes = [

	{
		path: '',
		loadComponent: () =>
			import('./pages/home-page/home-page.component').then((m) => m.HomePageComponent)
	},
	{
		path: 'devguessr',
		loadComponent: () =>
			import('./features/devguessr/pages/devguessr-page.component').then((m) => m.DevGuessrPageComponent)
	},
	{
		path: 'logodle',
		loadComponent: () =>
			import('./features/logodle/pages/logodle-page.component').then((m) => m.LogodlePageComponent)
	},
	{
		path: 'technections',
		loadComponent: () =>
			import('./features/technections/pages/technections-page.component').then(
				(m) => m.TechnectionsPageComponent
			)
	},
	{
		path: 'archive',
		loadComponent: () =>
			import('./pages/archive-page/archive-page.component').then((m) => m.ArchivePageComponent)
	},
	{
		path: 'about',
		loadComponent: () =>
			import('./pages/about-page/about-page.component').then((m) => m.AboutPageComponent)
	},
	{
		path: 'docs',
		loadComponent: () =>
			import('./pages/docs-page/docs-page.component').then((m) => m.DocsPageComponent)
	},
	{
		path: '**',
		redirectTo: ''
	}
];
