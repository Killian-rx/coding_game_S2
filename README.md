# coding_game_S2

# Morpion avec Serveur et Clients

Ce projet implémente un jeu de morpion avec un serveur et deux clients utilisant des sockets et une interface graphique en C#.

## Prérequis
- .NET SDK installé (version 6.0 ou supérieure).
- Les fichiers `cross.png` et `circle.png` doivent être placés dans le dossier du projet.

## Instructions pour lancer le projet

1. **Compiler le projet**  
   Ouvrez un terminal dans le dossier du projet et exécutez la commande suivante :  
   ```bash
   dotnet build
   ```

2. **Exécuter le projet**  
   Après la compilation, lancez le projet avec la commande suivante :  
   ```bash
   dotnet run
   ```

   Cela démarre :
   - Le serveur en arrière-plan.
   - Deux fenêtres clients (`Client1Form` et `Client2Form`) pour jouer au morpion.

## Fonctionnalités
- **Serveur** : Gère les mouvements des joueurs, vérifie les victoires et les matchs nuls.
- **Clients** : Affichent le plateau de jeu, les croix et les ronds, ainsi que le statut du jeu (victoire, match nul, ou tour actuel).

## Notes
- Si le projet ne se lance pas, vérifiez que tous les fichiers nécessaires (`Server.cs`, `Client.cs`, `Client1.cs`, `Client2.cs`) sont présents et correctement configurés.
- Les images `cross.png` et `circle.png` doivent être dans le dossier de sortie (`bin/Debug/net6.0-windows`).