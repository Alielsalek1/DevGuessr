import { Routes } from '@angular/router';

export const routes: Routes = [

	{
		path: '',
		loadComponent: () =>
			import('./pages/home-page/home-page.component').then((m) => m.HomePageComponent),
		data: { 
			title: 'DevGuessr - The Ultimate Developer Guessing Game', 
			description: 'Test your programming knowledge with DevGuessr. Guess the tech stack, frameworks, and syntax in this daily challenge.' 
		}
	},
	{
		path: 'langdle',
		loadComponent: () =>
			import('./features/langdle/pages/langdle-page.component').then((m) => m.LangdlePageComponent),
		data: { 
			title: 'Langdle - DevGuessr', 
			description: 'Guess the programming language from code snippets in Langdle. A daily puzzle for developers.' 
		}
	},
	{
		path: 'logodle',
		loadComponent: () =>
			import('./features/logodle/pages/logodle-page.component').then((m) => m.LogodlePageComponent),
		data: { 
			title: 'Logodle - DevGuessr', 
			description: 'Can you guess the developer tool or framework from its logo? Play Logodle daily.' 
		}
	},
	{
		path: 'mythdle',
		loadComponent: () =>
			import('./features/mythdle/pages/mythdle-page.component').then((m) => m.MythdlePageComponent),
		data: { 
			title: 'Mythdle - DevGuessr', 
			description: 'Is it real or a myth? Test your knowledge of tech products and frameworks in Mythdle.' 
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
		path: '**',
		redirectTo: ''
	}
];
