# **ARI2: Création d'une Application d'Analyse de Sentiment avec ML.NET**

## **Introduction**
Ce projet consiste à créer une application web interactive permettant d'analyser le sentiment d'un texte en utilisant ML.NET, un framework d'apprentissage automatique pour .NET. Cette application prend un commentaire utilisateur, prédit si le sentiment est positif ou négatif et affiche le résultat sur une interface web.

---

## **Prérequis**
Avant de commencer, assurez-vous que les éléments suivants sont installés et configurés sur votre machine :

- **.NET SDK**  
- **Visual Studio Code**  
- **Fichier de données** contenant des commentaires étiquetés avec des sentiments (positif/négatif). Le dataset est fourni dans le fichier `commentdata.txt` dans ce repository.

---

## **Étapes pour la Construction**

## 1. Télécharger les Données
Le fichier `commentdata.txt` contient des commentaires annotés avec des sentiments, où chaque ligne correspond à un commentaire et un label de sentiment (1 = positif, 0 = négatif).  
- **Origine du dataset** : [Microsoft Learn](https://learn.microsoft.com/).

---

## 2. Préparer l'Environnement

#### a. Vérifiez si .NET est installé
Utilisez la commande suivante dans votre terminal pour vérifier la version de .NET :
```bash
dotnet --version
```
### b.  Si dotnet n'est pas présent, installez le en suivant  :

```
    wget https://dot.net/v1/dotnet-install.sh
```

```
    bash dotnet-install.sh --channel 8.0 --install-dir ~/dotnet
```
```
    export PATH=$PATH:~/dotnet
```
```bash
    source ~/.bashrc
```
## 3.  Allez sur VSCode et créez un nouveau projet dotnet:
```bash
    dotnet new webapi -n AnalyseSentiment
```
```
    cd AnalyseSentiment
```

## 4.  Ajoutez la dépendance ML.NET au projet :
```
    dotnet add package Microsoft.ML --version 2.0.0
``` 

Vérifiez que le fichier  'AnalyseSentiment.csproj 'contient cette ligne dans la section <ItemGroup> pour inclure ML.NET, sinon ajoutez la. 

```xml
    <PackageReference Include="Microsoft.ML" Version="2.0.0" />
```
* * * * *

## 5. Structuter le Projet et ajouter les classes 
Naviguez dans le dossier du projet et organisez les fichiers nécessaires, en suivant cette structure.

```bash

AnalyseSentiment/ 
│
├── Data/                        # Dossier contenant les données
│   └── commentdata.txt          # Fichier de données des commentaires annotés
│
├── Models/                      # Contient les classes de données et de prédiction
│   ├── SentimentData.cs         # Classes SentimentData et SentimentPrediction
│
├── Pages/                       # Pages Razor pour l'interface web
│   ├── Index.cshtml             # Page principale où les utilisateurs saisissent des commentaires
│   ├── Index.cshtml.cs          # Code-behind pour la page principale
│
├── Services/                    # Services de l'application pour la logique réutilisable
│   ├── SentimentAnalyzer.cs     # Contient la logique pour l'entraînement et la prédiction des sentiments
│
├── wwwroot/                     # Fichiers statiques pour l'application web
│   ├── css/
│   │   └── site.css             # Styles personnalisés pour l'interface web
│
├── Properties/                  # Répertoire de configuration
│   └── launchSettings.json      # Configuration pour l'exécution de l'application
│
├── AnalyseSentiment.csproj      # Fichier de projet
│
├── Program.cs                   # Point d'entrée de l'application
│
└── appsettings.json             # Configuration de l'application

```


#### **5.1. Dossier pour les Données**  
- Créez un répertoire `Data` pour stocker les fichiers nécessaires.  
- Copiez le fichier de données dans ce répertoire.  

#### **5.2. Classe SentimentData **  
- Dans le dossier Models, ajoutez une classe pour représenter les données et les prédictions (colonnes pour le texte, label, probabilité, etc.).  

#### **5.3. Service **  
- Implémentez `SentimentService.cs` pour encapsuler la logique :  
  - Charger les données.  
  - Diviser en ensembles d'entraînement et de test.  
  - Construire et entraîner le modèle.  
  - Ajouter la prédiction.  
---

## 6. Ajouter une Interface Web 

#### **6.1. Fichiers HTML et Razor**  

- **`Index.cshtml` :**  
  - Formulaire permettant à l’utilisateur de saisir un commentaire.  
  - Affichage du résultat de l’analyse de sentiment.  

- **`Index.cshtml.cs` :**  
  - Logique côté serveur pour traiter le formulaire.  
  - Connexion avec `SentimentService` pour effectuer les prédictions.  

#### **6.2. CSS pour le Frontend**  
- **`site.css` :**  
  - Ajoutez du style pour rendre l'interface utilisateur conviviale et attrayante.  

---

### **7. Compiler et Exécuter**  
- Compilez le projet avec `dotnet build` pour vérifier les erreurs.  
- Lancez l'application avec `dotnet run`.  
- Accédez à l'application via le navigateur à l'URL affichée dans la console.  


**Et un grand MERCI !**
