# Explication du setup STS2 partagé

Ce fichier explique ce qui a été préparé sur le VPS pour développer un mod de personnage Slay the Spire 2 sans dupliquer les gros outils dans chaque compte utilisateur.

## Objectif

Le but est que `alex` et `david` puissent développer chacun leur mod STS2 en utilisant les mêmes installations partagées :

- le jeu Slay the Spire 2 ;
- le SDK `.NET` ;
- Godot / les outils Godot nécessaires au packaging ;
- un template de mod de personnage qui reprend l'architecture de Hologirl, mais sans contenu spécifique à Hologirl.

Le volume principal `/` était presque plein. Les éléments lourds ont donc été déplacés vers le disque monté dans :

```sh
/mnt/HC_Volume_105232828
```

## Arborescence partagée

Les outils partagés sont sous :

```sh
/mnt/HC_Volume_105232828/shared
```

Structure actuelle :

```sh
/mnt/HC_Volume_105232828/shared/games/slay-the-spire-2
/mnt/HC_Volume_105232828/shared/tools/dotnet
/mnt/HC_Volume_105232828/shared/tools/godot
/mnt/HC_Volume_105232828/shared/cache
```

Ces dossiers sont prévus pour être accessibles en écriture par les utilisateurs membres du groupe `sudo`, donc par `alex` et `david`.

## Liens symboliques recommandés pour David

Les commandes suivantes créent les liens locaux dans le compte `david`.

À lancer depuis n'importe où :

```sh
sudo -u david ln -s "/mnt/HC_Volume_105232828/shared/tools/dotnet" "/home/david/.dotnet"
sudo -u david mkdir -p "/home/david/games" "/home/david/.cache"
sudo -u david ln -s "/mnt/HC_Volume_105232828/shared/games/slay-the-spire-2" "/home/david/games/slay-the-spire-2"
sudo -u david ln -s "/mnt/HC_Volume_105232828/shared/tools/godot" "/home/david/.cache/sts2-tools"
```

Si un lien existe déjà, vérifier avant de supprimer. Ne pas écraser un dossier contenant du travail.

Vérification :

```sh
ls -ld /home/david/.dotnet /home/david/games/slay-the-spire-2 /home/david/.cache/sts2-tools
sudo -u david /home/david/.dotnet/dotnet --list-sdks
```

## Variables d'environnement utiles

Dans le shell de David, ces variables permettent aux scripts de trouver les bons outils :

```sh
export DOTNET_ROOT=/mnt/HC_Volume_105232828/shared/tools/dotnet
export PATH="$DOTNET_ROOT:$PATH"
export STS2_DIR=/mnt/HC_Volume_105232828/shared/games/slay-the-spire-2
export GODOT_BIN=/mnt/HC_Volume_105232828/shared/tools/godot/godot-4.5.1/Godot_v4.5.1-stable_mono_linux_x86_64/Godot_v4.5.1-stable_mono_linux.x86_64
```

David peut les ajouter à son `.bashrc` ou `.zshrc` si elles sont utilisées souvent.

## Template de mod

Un template barebone a été préparé dans :

```sh
/tmp/sts2-character-template
```

Il reprend la structure technique de Hologirl, mais le contenu spécifique à Hologirl a été retiré :

- pas d'archives d'art Hologirl ;
- pas de cartes Hologirl ;
- pas de pouvoirs/reliques Hologirl ;
- pas de direction artistique Hologirl ;
- nom générique `TemplateCharacter` ;
- documentation gardée volontairement sparse.

Pour copier ce template vers le projet de David :

```sh
sudo cp -R /tmp/sts2-character-template /home/david/Sts2/template
sudo chown -R david:david /home/david/Sts2/template
```

Si `/tmp/sts2-character-template` n'existe plus, il faut le régénérer depuis le repo Hologirl ou le recopier depuis une archive.

## Développer son personnage

Workflow conseillé :

1. Copier le template dans un nouveau dossier de projet.
2. Renommer les fichiers, namespaces, manifest et IDs du personnage.
3. Garder les IDs uniques pour éviter les collisions avec d'autres mods.
4. Ajouter les cartes, reliques, pouvoirs et assets progressivement.
5. Tester localement avec un build minimal avant d'ajouter beaucoup de contenu.

Les scripts importants du template sont normalement :

```sh
scripts/build.sh
scripts/package.sh
scripts/release.sh
```

Usage typique :

```sh
./scripts/build.sh
./scripts/package.sh
```

Le script de release ne doit être utilisé que quand le mod est prêt à être publié sur GitHub.

## Bonnes pratiques STS2

Slay the Spire 2 et son écosystème de modding évoluent vite. Avant de prendre une décision importante sur l'API de modding, les dépendances, le loader, BaseLib, le packaging ou le format des manifests, vérifier l'état actuel dans des sources vivantes :

- GitHub de mods qui fonctionnent ;
- GitHub des templates STS2 actifs ;
- Nexus Mods ;
- source code des dépendances comme BaseLib ;
- logs du jeu.

Ne pas supposer que les informations STS1 ou les anciens exemples STS2 sont encore valables.

## Compatibilité avec les autres mods

Pour éviter les collisions :

- utiliser un préfixe unique pour les IDs internes ;
- éviter les noms génériques globaux ;
- ne pas patcher le comportement vanilla plus largement que nécessaire ;
- déclarer les dépendances de façon compatible avec le manifest attendu par STS2 ;
- ne pas embarquer les DLL de dépendances si le loader s'attend à les charger comme mods séparés ;
- tester avec BaseLib et d'autres mods activés.

## Notes sur le disque

Le disque principal était presque plein. Les gros éléments ont été déplacés vers le disque `/mnt`.

État attendu après déplacement :

- `/home/alex/games/slay-the-spire-2` est un lien vers le volume partagé ;
- `/home/alex/.dotnet` est un lien vers le volume partagé ;
- `/home/alex/.cache/hologirl-tools` est un lien vers le volume partagé ;
- David devrait avoir des liens équivalents.

Pour inspecter l'espace disque :

```sh
df -h
du -h --max-depth=1 /home/david 2>/dev/null | sort -h
du -h --max-depth=1 /home/alex 2>/dev/null | sort -h
```

Éviter de réinstaller Steam, Godot ou dotnet dans chaque home utilisateur si les versions partagées conviennent.

## Commande pour installer ce fichier

Ce fichier a été écrit dans :

```sh
/tmp/explication.md
```

Pour le placer dans le projet de David :

```sh
sudo cp /tmp/explication.md /home/david/Sts2/Elesis/explication.md
sudo chown david:david /home/david/Sts2/Elesis/explication.md
```
