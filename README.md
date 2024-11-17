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
├── Data/                        
│   └── commentdata.txt          
│
├── Models/                      
│   ├── SentimentData.cs        
│
├── Controllers/                       
│   ├── SentimentControllers.cs     
│ 
│
├── Services/                   
│   ├── SentimentService.cs      
│
├── wwwroot/                     
│   ├── css/
│   │   └── styles.css     
│   ├── js/
│   │   └── app.js     
│   │   └── index.html         
│
├── Properties/                  
│   └── launchSettings.json      
│
├── AnalyseSentiment.csproj      
│
├── Program.cs                   
│
└── appsettings.json             

```


---

### **6. Compiler et Exécuter**  
- Compilez le projet avec `dotnet build` pour vérifier les erreurs.  
- Lancez l'application avec `dotnet run`.  



### **7. Tester**  
```bash
curl -X POST http://localhost:5121/api/sentiment/predict \
-H "Content-Type: application/json" \
-d "\"I love this product! It's amazing.\""

```



**Et un grand MERCI !**
