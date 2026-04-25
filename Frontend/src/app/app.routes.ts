import { Routes } from '@angular/router';

export const routes: Routes = [

	{
		path: '',
		loadComponent: () =>
			import('./pages/home-page/home-page.component').then((m) => m.HomePageComponent),
		data: {
			title: 'DevGuessr | Daily Puzzle Game for Developers',
			description: 'Master the ultimate developer challenge. Solve daily puzzles in programming syntax, technical logos, and software lore. New challenges drop at midnight UTC.'
		}
	},
	{
		path: 'langdle',
		loadComponent: () =>
			import('./features/langdle/pages/langdle-page.component').then((m) => m.LangdlePageComponent),
		data: {
			title: 'Langdle | Programming Language Guessing Game',
			description: 'Identify the secret programming language using technical feedback on year, typing, and syntax. New daily challenge every 24 hours.'
		}
	},
	{
		path: 'logodle',
		loadComponent: () =>
			import('./features/logodle/pages/logodle-page.component').then((m) => m.LogodlePageComponent),
		data: {
			title: 'Logodle | Tech Logo Visual Challenge',
			description: 'Can you identify the developer tool or framework logo? Test your visual recognition in this daily pixelated logo game.'
		}
	},
	{
		path: 'mythdle',
		loadComponent: () =>
			import('./features/mythdle/pages/mythdle-page.component').then((m) => m.MythdlePageComponent),
		data: {
			title: 'Mythdle | Developer Fact or Fiction',
			description: 'Spot the fabrication among real technical concepts. A daily logic game to test your knowledge of the software ecosystem.'
		}
	},
	{
		path: 'past-drops',
		loadComponent: () =>
			import('./pages/archive-page/archive-page.component').then((m) => m.ArchivePageComponent),
		data: {
			title: 'Past Drops - DevGuessr',
			description: 'Play previous DevGuessr challenges and catch up on any puzzles you missed.'
		}
	},
	{
		path: 'about',
		loadComponent: () =>
			import('./pages/about-page/about-page.component').then((m) => m.AboutPageComponent),
		data: {
			title: 'About - DevGuessr',
			description: 'Learn more about DevGuessr, the ultimate suite of daily developer puzzle games.'
		}
	},
	{
		path: 'docs',
		loadComponent: () =>
			import('./pages/docs-page/docs-page.component').then((m) => m.DocsPageComponent),
		data: {
			title: 'Docs - DevGuessr',
			description: 'Documentation and guides for DevGuessr.'
		}
	},
	{
		path: 'privacy',
		loadComponent: () =>
			import('./pages/privacy-page/privacy-page.component').then((m) => m.PrivacyPageComponent),
		data: {
			title: 'Privacy Policy - DevGuessr',
			description: 'Privacy Policy for DevGuessr.'
		}
	},
	{
		path: 'terms',
		loadComponent: () =>
			import('./pages/terms-page/terms-page.component').then((m) => m.TermsPageComponent),
		data: {
			title: 'Terms of Use - DevGuessr',
			description: 'Terms of Use for DevGuessr.'
		}
	},
	{
		path: '**',
		redirectTo: ''
	}
];
