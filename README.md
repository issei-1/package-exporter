# Hierarchy Package Exporter

UnityのHierarchyで選択したGameObjectを、子オブジェクト・依存アセットと一緒に `.unitypackage` へ書き出すEditorツールです。C#スクリプト1本で動作し、追加ライブラリは不要です。

## 機能

- Hierarchyで選択した1つのオブジェクトをPrefab化して書き出し
- 子オブジェクトと、Unityが検出する依存アセットを同梱
- 保存ダイアログで保存先・ファイル名を指定
- キャンセル対応、完了後に保存先を表示
- パッケージ生成成功後に既存ファイルを置き換え
- 処理後に複製オブジェクト・一時Prefab・一時出力を削除

## 導入

### unitypackageから

1. [HierarchyPackageExporter.unitypackage](dist/HierarchyPackageExporter.unitypackage) をダウンロードします（GitHubではファイル画面の **Download raw file**）。
2. ダウンロードした[HierarchyPackageExporter.unitypackage](dist/HierarchyPackageExporter.unitypackage)をダブルクリックして、インポートします。

更新時は同じスクリプトを上書きしてください。

## 使い方

1. 必要なアセットの変更を保存します。
2. `Tools > Hierarchy Package Exporter` を開きます。
3. Hierarchyで書き出すGameObjectを1つ選択します。
4. **.unitypackage を書き出す** を押します。
5. 保存先とファイル名を指定します。

再生中は書き出せません。複数のオブジェクトをまとめたい場合は、1つの親オブジェクトの下に置き、その親を選択してください。

## 別プロジェクトで復元する

1. 必要な外部パッケージやSDKを導入します。
2. 書き出した `.unitypackage` をインポートします。
3. `Assets/HierarchyExport_<オブジェクト名>_<ID>.prefab` をHierarchyへドラッグします。

参照は名前ではなくGUIDなどの識別情報で結び付けられます。同名のアセットがあるだけでは互換性を保証できません。既存アセットを更新する場合は、Unityのインポート一覧を確認してください。

## 同梱されるもの・制約

`AssetDatabase.ExportPackage` の `IncludeDependencies` を使用します。マテリアル、テクスチャ、モデル、シェーダー、アニメーションクリップ、スクリプトなど、Unityが依存関係として検出するアセットが対象です。

- 依存関係によっては、想定より多くのアセットやスクリプトが含まれることがあります。
- 名前・パスで動的に読み込むアセットは自動検出されない場合があります。
- 未保存の生成Mesh・Materialなどは、事前にアセットとして保存してください。
- 選択対象の外にあるシーンオブジェクトへの参照、シーン全体の設定、Project Settingsは復元対象ではありません。
- URP/HDRPや外部SDKなどのパッケージは、移行先でも別途導入してください。
- Unityやレンダーパイプライン、シェーダーのバージョンが異なると表示・動作が変わる場合があります。
- 複製時に `ExecuteAlways` などのEditor実行スクリプトが動作する場合があります。
- 上書きには `.NET File.Replace` を使用します。保存先のファイルシステムが非対応の場合は、新しいファイル名で保存してください。

## 対応環境・検証状況

Unity 2019.4以降を想定していますが、Unity Editorでのコンパイル・動作確認は未実施です。対応バージョンを保証するものではありません。

配布用 `.unitypackage` の展開と、格納したスクリプトが公開ソースと一致することは確認しています。

Unityで確認する際は、通常オブジェクト・Prefabインスタンス・子階層の書き出し、マテリアル等の参照、保存キャンセル、既存ファイルへの上書き、別プロジェクトへのインポートを確認してください。

## 開発・貢献

実装は `Assets/Editor/HierarchyPackageExporter.cs` にあります。変更後はUnityで確認し、`Assets > Export Package...` からツールのスクリプトを選んで `dist/HierarchyPackageExporter.unitypackage` を更新してください。

不具合報告にはUnityバージョン、OS、レンダーパイプライン、再現手順、Consoleのエラーを添えてください。Issue・Pull Requestを歓迎します。

## ライセンス

[MIT License](LICENSE)。このライセンスは本ツールに適用されます。書き出した他のアセットのライセンスは変更されません。

## API参考

- [AssetDatabase.ExportPackage](https://docs.unity3d.com/ScriptReference/AssetDatabase.ExportPackage.html)
- [PrefabUtility.SaveAsPrefabAsset](https://docs.unity3d.com/ScriptReference/PrefabUtility.SaveAsPrefabAsset.html)
- [EditorUtility.SaveFilePanel](https://docs.unity3d.com/ScriptReference/EditorUtility.SaveFilePanel.html)
