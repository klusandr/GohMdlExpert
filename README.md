## -- EN --

### Beta version of the program for creating skins of human units (humanksin) in the game "Call to arms - Gates of Hell"

### Installation:
  Unzip the archive wherever you want and run it.

### Versions:
   (beta)
   - 0.0.1.0 release
   - 0.0.2.0 add finnish
   - 0.0.2.1 hotfix default LOD null, load soviet resource, load unpack resource
   - 0.0.3.0 add united kingdom, any fix
   
### First launch:
  
  When you first launch the program, you will be asked to specify the path to the game folder, usually it is "D:\..\steamapps\common\Call to Arms - Gates of Hell". After specifying it, the program will download all the necessary resources and display the current version of the game.

### Operation:
    
  When the game resources are loaded, a drop-down list of factions (German, United states, Soviets) will be available in the left column at the top, select the desired one. The current version of the program cannot yet save skins with elements of different factions.
  
  The structure of the elements from which skin models can be composed is presented as a tree. To select an element (ply), double-click on it, it will turn green and be displayed in the display window, but will not yet be added to the skin model. When selecting an element, we will also see its meshes, the names of the textures (mtl) that this element uses. The current materials of each texture are highlighted in green. To add, you must click the "Add select model" button.
    
  After adding, the element will be displayed in the right column, in the current skin's overview tree. It displays the file structure, the model (mdl) and its textures (mtl).
  
  If you expand the model file (mdl), you can see what elements (ply) it consists of. By right-clicking on an element, you can remove it from the skin, hide it, or set focus on it. In the lower window of the right column, with a selected skin element, you can configure its LOD files. If you expand the element, we will see the meshes, which are the same textures, that the skin elements use. The current texture material is displayed in square brackets after the mesh name.
  
  If you select a texture file (mtl), the available materials for the selected texture are at the bottom of the right column. If you expand the texture file, the skin elements that use the current texture will be displayed.
  To add a new texture, click the "Add" button under the texture list overview window, and in the window that appears, in the "Textures" tab, select a texture suitable for all skin elements, and click "Ok". To edit an existing texture, double-click on it. To select a texture that will be used when saving, simply select it in the list. Carefully check that all elements adequately display the selected texture.
  
  To save the skin, click the "Save humanskin" button at the bottom of the right column, and select the location where you want to save it.
  
  :exclamation:Important:exclamation: The correct path for saving the skin should be the following "your_mod\resource\entity\humanskin\[faction_name]\your_folder\humanskin.mdl", and nothing else. At least in this version of the program.
    To open already created skins, select the "File" item in the top menu of the program, then click "Open file". The current version of the program cannot open skin files from the original game without unpacking them.
  
  :exclamation:Important:exclamation: Open the skin file only if it belongs to the faction selected in the program, otherwise errors will occur.

### Notes:
  
  When selecting skin elements to add them, they may be missing a texture, this usually occurs when this element has never been used in the game and the program does not know which texture to use. You can specify which textures will be used by default, if the element does not have one initially. To do this, right-click on the mesh (mtl) of the element, and select "Add default texture". Then in the window that opens, select the texture that suits the current element.
  
  If for some reason you do not have textures for any skin element, and there are no elements in the "Load materials" texture selection window, then you need to load the cache to display them. Select the item in the top menu "Edit->Caches", and "Load PLY model textures" if there are no textures for the elements,
  or "Load textures" if there are no textures in the selection window.

### You can report a bug here https://forms.gle/1pJpmXkk4RSkcXfY9

### Since this is only a beta version, there may be a ton of bugs and shortcomings, so I count on your help in developing this program.

### Roadmap:
- [ ] Connecting resources from modifications along with game ones;
- [ ] Loading existing skins from the game without unpacking them;
- [ ] Ability to combine elements of different factions;
- [ ] More flexibility in saving skins;
- [ ] Localization of the application to EN, RU and DE.

### Current version 0.0.3.0

## -- RU --

### Бета версия программы для создания скинов человеческих юнитов (humanksin) в игре "Call to arms - Gates of Hell"

### Версии:
   (beta)
   - 0.0.1.0 релиз
   - 0.0.2.0 долбавлены фины
   - 0.0.2.1 hotfix стандартного LOD null, загрузки советских ресурсов, загрузки распакованных ресурсов
   - 0.0.3.0 добавлено соедиенённое королевство, другие исправления

### Установка:

  Распакуйте архив куда хотите, и запускайте.

### Первый запуск: 

  При первом запуске программа попросит вас указать путь к папке игры, обычно это "D:\..\steamapps\common\Call to Arms - Gates of Hell". После его указания, программа загрузит все необходимые ресурсы и отобразит текущую версию игры.

### Работа:

  Когда ресурсы игры загружены, в левой колонке сверху будет доступен выпадающий список фракций (German, United states, Soviets), выбираем нужную. Текущая версия программы пока не умеет сохранять скины с элементами разных фракций.
  
  Структура элементов, из которых можно составлять модели скинов, представлена в виде дерева. Для выбора элемента (ply), нажимаем на него дважды, он перекрасится в зелёный и отобразится в окне отображения, однако ещё не будет добавлен в модель скина. При выборе элемента, мы так же увидим его меши, названия текстур (mtl), которые использует этот элемент. Текущие материалы каждой текстуры подсвечены зелёным. Для добавления элемента в скин необходимо нажать кнопку "Add select model". 
    
  После добавления, элемент отобразится в правой колонке, в дереве обзора текущего скина. В нём отображается структура файлов, модель(mdl) и её текстуры(mtl).
    
  Если развернуть файл модели (mdl), можно увидеть из каких элементов(ply) она состоит. Через нажатие на элемент правой кнопкой мыши можно удалить его из скина, скрыть, или установить на него фокус. В нижнем окне правой колонки, при выделенном элементе скина, можно настроить его LOD файлы. Если развернуть элемент, мы увидим меши, они же текстуры, которые используют элементы скина. Текущий материал текстуры отображается в квадратных скобках после названия меша.

  Если выбрать файл текстуры (mtl), внизу правой колонки доступные материалы для выделенной текстуры. Если её развернуть, отобразиться элементы скина, использующие текущую текстуру. 
Для добавления новой текстуры, нажимаем кнопку "Add" под окном обзора списка текстур, и в появившемся окне, во вкладке "Textures", выбираем текстуру подходящую для всех элементов скина, и нажимаем "Ok". Для редактирования
уже существующей текстуры, дважды на неё нажимаем. Что бы выбрать текстуру, которая будет использована при сохранении, просто выбираем её в списке. Внимательно проверяйте, что все элементы адекватно отображают выбранную текстуру.

  Для сохранения скина, нажимаем кнопку "Save humanskin" внизу правой колонки, и выбираем расположение, куда хотим его сохранить.
  
  :exclamation:Важно:exclamation: Корректный путь для сохранения скина должен быть следущим "ваш_мод\resource\entity\humanskin\[имя_фракции]\ваша_папка\humanskin.mdl", и никакой иначе. Во всяком случае в данной версии программы.
    
  Для открытия уже созданных скинов, в выбираем пункт "File" в верхнем меню программы, после нажимаем "Open file". Текущая версия программы не может открывать файлы скинов из оригинальной игры без их распаковки.
  
  :exclamation:Важно:exclamation: Открываем файл скина, только если он принадлежит выбранной в программе фракции, иначе возникнут ошибки.

Примечания:
    
  При выборе элементов скинов для их добавления, у них может отсутствовать текстура, обычно это возникает, когда данный элемент ни разу не использовался в игре и программа не знает какую текстуру использовать. Можно указать, какие текстуры будут использованы по умолчанию, если элемент не имеет её изначально. Для этого нажимаем правой кнопки мыши по мешу (mtl) элемента, и выбираем пункт "Add default texture". После в открывшемся окне выбираем текстуру, которая подходит для текущего элемента. 
  
  Если у вас по какой-то причине ни для одного элемента скина нет текстур, а в окне выбора текстур "Load materials" нет элементов, то необходимо загрузить кэш для их отображения. Выбираем пункт в верхнем меню "Edit->Caches", и "Load PLY model textures" если нет текстур для элементов,
  или "Load textures" если нету текстур в окне их выбора.

### Сообщить об ошибке можно тут https://forms.gle/1pJpmXkk4RSkcXfY9

### Так как это только бета версия, ошибок и недочётов может быть вагон и ещё грузовик на подходе, поэтому рассчитываю на вашу помощь в развитии этой программы. 

### Дорожная карта:
- [ ] Подключение ресурсов из модификаций вместе с игровыми;
- [ ] Загрузка существующих скинов из игры без их распаковки;
- [ ] Возможность совмещать элементы разных фракций;
- [ ] Больше гибкости в сохранении скинов;
- [ ] Локализация приложения на EN, RU и DE.

### Текущая версия 0.0.3.0
