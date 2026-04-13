import { Routes } from '@angular/router';

export const routes: Routes = [

	{
		path: '',
		loadComponent: () =>
			import('./pages/home-page/home-page.component').then((m) => m.HomePageComponent)
	},
	{
		path: 'langdle',
		loadComponent: () =>
			import('./features/langdle/pages/langdle-page.component').then((m) => m.LangdlePageComponent)
	},
	{
		path: 'logodle',
		loadComponent: () =>
			import('./features/logodle/pages/logodle-page.component').then((m) => m.LogodlePageComponent)
	},
	{
		path: 'clusterdle',
		loadComponent: () =>
			import('./features/clusterdle/pages/clusterdle-page.component').then(
				(m) => m.ClusterdlePageComponent
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
