/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/b-b-blueberry/CooksAssistant
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using Object = StardewValley.Object;

namespace LoveOfCooking
{
	public class AssetManager : IAssetEditor, IAssetLoader
	{
		private static Config Config => ModEntry.Instance.Config;
		private static ITranslationHelper i18n => ModEntry.Instance.Helper.Translation;

		private static readonly Dictionary<string, int> BuffIndex = new Dictionary<string, int>
		{
			{ "Farming", 0},
			{ "Fishing", 1},
			{ "Mining", 2},
			{ "Luck", 4},
			{ "Foraging", 5},
			{ "Energy", 7},
			{ "Magnetism", 8},
			{ "Speed", 9},
			{ "Defense", 10},
			{ "Attack", 11},
			{ "Cooking", 12},
		};
		internal static readonly Point IconPosition = new Point(506, 372);

		public AssetManager() {}

		public bool CanLoad<T>(IAssetInfo asset)
		{
			return asset.AssetNameEquals(ModEntry.GameContentSpriteSheetPath)
				|| asset.AssetNameEquals(ModEntry.GameContentBundleDataPath)
				|| asset.AssetNameEquals(ModEntry.MapTileSheetPath);
		}

		// TODO: UPDATE: Add context tags to all new objects

		public T Load<T>(IAssetInfo asset)
		{
			Log.D($"Loading custom asset {asset.AssetName}",
				ModEntry.Instance.Config.DebugMode);
			if (asset.AssetNameEquals(ModEntry.GameContentSpriteSheetPath))
			{
				return (T) (object) ModEntry.Instance.Helper.Content.Load<Texture2D>($"{ModEntry.LocalSpriteSheetPath}.png");
			}
			if (asset.AssetNameEquals(ModEntry.GameContentBundleDataPath))
			{
				return (T) (object) ModEntry.Instance.Helper.Content.Load<Dictionary<string, List<List<string>>>>($"{ModEntry.LocalBundleDataPath}.json");
			}
			if (asset.AssetNameEquals(ModEntry.MapTileSheetPath))
			{
				// Take bar counter sprite from townInterior sheet
				var sourceImage = Game1.content.Load<Texture2D>(@"Maps/townInterior");
				var sourceArea = new Rectangle(352, 448, 16, 16);
				// Take plate sprite from custom mapSprites sheet
				var destImage = ModEntry.Instance.Helper.Content.Load<Texture2D>($"{ModEntry.MapTileSheetPath}.png");
				var destArea = new Rectangle(0, 0, 16, 16);

				// Take plate pixels
				var platePixels = new Color[destArea.Width * destArea.Height];
				destImage.GetData(0, destArea, platePixels, 0, platePixels.Length);
				// Take bar counter pixels
				var barPixels = new Color[sourceArea.Width * sourceArea.Height];
				sourceImage.GetData(0, sourceArea, barPixels, 0, platePixels.Length);
				// Overlay plate pixels onto bar pixels
				for (var row = 0; row < destArea.Height; ++row)
				for (var column = 0; column < destArea.Width; ++column)
				{
					var index = row + column * destArea.Width;
					if (platePixels[index].A > 0)
						barPixels[index] = platePixels[index];
				}
				// Replace old pixels with new plate-on-bar pixels
				destImage.SetData(0, destArea, barPixels, 0, barPixels.Length);

				// Apply sprite changes
				return (T) (object) destImage;
			}
			return (T) (object) null;
		}

		public bool CanEdit<T>(IAssetInfo asset)
		{
			return Game1.player != null 
			    && (asset.AssetNameEquals(@"Data/Bundles")
			        || asset.AssetNameEquals(@"Data/BigCraftablesInformation")
			        || asset.AssetNameEquals(@"Data/CookingRecipes")
			        || asset.AssetNameEquals(@"Data/mail")
					|| asset.AssetNameEquals(@"Data/ObjectInformation")
			        || asset.AssetNameEquals(@"Data/Events/Saloon")
			        || asset.AssetNameEquals(@"Data/Events/Mountain")
			        || asset.AssetNameEquals(@"Data/Events/JoshHouse")
					|| asset.AssetNameEquals(@"LooseSprites/Cursors")
					|| asset.AssetNameEquals(@"LooseSprites/JunimoNote")
			        || asset.AssetNameEquals(@"Maps/Beach")
			        || asset.AssetNameEquals(@"Maps/springobjects")
			        || asset.AssetNameEquals(@"Maps/townInterior")
			        || asset.AssetNameEquals(@"Strings/UI")
			        || asset.AssetNameEquals(@"Strings/Locations")
					|| asset.AssetNameEquals(@"TileSheets/tools"));
		}

		public void Edit<T>(IAssetData asset)
		{
			EditAsset(ref asset); // eat that, ENC0036
		}

		private void EditAsset(ref IAssetData asset)
		{
			if (ModEntry.PrintRename)
				Log.D($"Editing {asset.AssetName}",
					ModEntry.Instance.Config.DebugMode);
			if (asset.AssetNameEquals(@"Data/Bundles"))
			{
				// Make no changes for the new community centre bundle, but set our base values from the data
				// Do this even with community centre changes disabled, in case we join as farmhand to a player who has it enabled

				if (!Game1.hasLoadedGame)
				{
					return;
				}

				var data = asset.AsDictionary<string, string>().Data;
				ModEntry.BundleStartIndex = 1 + data.Keys.ToList().Max(key => int.Parse(key.Split('/')[1]));
			}
			else if (asset.AssetNameEquals(@"Data/BigCraftablesInformation"))
			{
				var data = asset.AsDictionary<int, string>().Data;

				// Add localised names for new craftables
				foreach (var pair in data.Where(pair => pair.Value.Split('/')[0].StartsWith(ModEntry.ObjectPrefix)).ToList())
				{
					var split = pair.Value.Split('/');
					var name = split[0].Split(new[] { '.' }, 3);
					var nameData = data[pair.Key];
					split[4] = i18n.Get($"item.{name[2]}.description").ToString();
					split[8] = i18n.Get($"item.{name[2]}.name").ToString();
					data[pair.Key] = ModEntry.SplitToString(split);
					if (ModEntry.PrintRename)
						Log.D($"Named craftable {name[2]} ({data[pair.Key].Split('/')[5]})", ModEntry.Instance.Config.DebugMode);
				}

				asset.AsDictionary<int, string>().ReplaceWith(data);
			}
			else if (asset.AssetNameEquals(@"Data/CookingRecipes"))
			{
				// Edit fields of vanilla recipes to use new ingredients

				if (ModEntry.JsonAssets == null || Game1.currentLocation == null)
					return;

				var data = asset.AsDictionary<string, string>().Data;

				// Add localised names for new recipes
				// While this also happens in CookingMenu.ctor for English locales, it's efficient here for other locales
				foreach (var pair in data.Where(pair => pair.Key.StartsWith(ModEntry.ObjectPrefix)).ToList())
				{
					var name = pair.Key.Split(new[] { '.' }, 3);
					var nameData = data[pair.Key];
					var split = new string[6];
					data[pair.Key].Split('/').CopyTo(split, 0);
					split[5] = i18n.Get($"item.{name[2]}.name").ToString();
					data[pair.Key] = ModEntry.SplitToString(split);
					if (ModEntry.PrintRename)
						Log.D($"Named recipe {name[2]} ({data[pair.Key].Split('/')[5]})", ModEntry.Instance.Config.DebugMode);
				}
				asset.AsDictionary<string, string>().ReplaceWith(data);

				if (!Config.AddRecipeRebalancing)
				{
					Log.D($"Did not edit {asset.AssetName}: New recipe scaling is disabled in config file.",
						Config.DebugMode);
					return;
				}

				try
				{
					var recipeData = new Dictionary<string, string>
					{
						// Maki Roll: Sashimi 1 Seaweed 1 Rice 1
						{
							"Maki Roll",
							"227 1 152 1 423 1"
						},
						// Coleslaw: Vinegar 1 Mayonnaise 1
						{
							"Coleslaw",
							$"{ModEntry.JsonAssets.GetObjectId(ModEntry.ObjectPrefix + "cabbage")} 1"
							+ " 419 1 306 1"
						},
						// Pink Cake: Cake 1 Melon 1
						{
							"Pink Cake",
							$"{ModEntry.JsonAssets.GetObjectId(ModEntry.ObjectPrefix + "cake")} 1"
							+ " 254 1"
						},
						// Chocolate Cake: Cake 1 Chocolate Bar 1
						{
							"Chocolate Cake",
							$"{ModEntry.JsonAssets.GetObjectId(ModEntry.ObjectPrefix + "cake")} 1" 
							+ $" {ModEntry.JsonAssets.GetObjectId(ModEntry.ObjectPrefix + "chocolate")} 1"
						},
						// Cookies: Flour 1 Category:Egg 1 Chocolate Bar 1
						{
							"Cookies",
							"246 1 -5 1"
							+ $" {ModEntry.JsonAssets.GetObjectId(ModEntry.ObjectPrefix + "chocolate")} 1"
						},
						// Pizza: Flour 2 Tomato 2 Cheese 2
						{
							"Pizza",
							"246 2 256 2 424 2"
						},
					};
					foreach (var recipe in recipeData)
						data[recipe.Key] = ModEntry.UpdateEntry(data[recipe.Key], new [] {recipe.Value});

					// Substitute in the actual custom ingredients for custom recipes if custom objects are enabled
					if (ModEntry.Instance.Config.AddNewCropsAndStuff
						&& ModEntry.Instance.Config.AddCookingSkillAndRecipes)
					{
						recipeData = new Dictionary<string, string>
						{
							// Beet Burger: Bread 1 Beet 1 Onion 1 Red Cabbage 1
							{
								ModEntry.ObjectPrefix + "burger",
								"216 1 284 1"
								+ $" {ModEntry.JsonAssets.GetObjectId(ModEntry.ObjectPrefix + "onion")} 1"
								+ " 266 1"
							},
							// Cabbage Pot: Cabbage 2 Onion 2
							{
								ModEntry.ObjectPrefix + "cabbagepot",
								$"{ModEntry.JsonAssets.GetObjectId(ModEntry.ObjectPrefix + "cabbage")} 2"
								+ $" {ModEntry.JsonAssets.GetObjectId(ModEntry.ObjectPrefix + "onion")} 2"
							},
							// Garden Pie: Flour 1 Cabbage 1 Onion 1 Tomato 1
							{
								ModEntry.ObjectPrefix + "gardenpie",
								"246 1"
								+ $" {ModEntry.JsonAssets.GetObjectId(ModEntry.ObjectPrefix + "cabbage")} 1"
								+ $" {ModEntry.JsonAssets.GetObjectId(ModEntry.ObjectPrefix + "onion")} 1"
								+ " 256 1"
							},
							// Hearty Stew: Carrot 2 Potato 1
							{
								ModEntry.ObjectPrefix + "stew",
								$"{ModEntry.JsonAssets.GetObjectId(ModEntry.ObjectPrefix + "carrot")} 2"
								+ " 192 1"
							},
							// Hot Cocoa: Milk (Any) 1 Chocolate Bar 1
							{
								ModEntry.ObjectPrefix + "hotcocoa",
								"-6 1"
								+ $" {ModEntry.JsonAssets.GetObjectId(ModEntry.ObjectPrefix + "chocolate")} 1"
							},
							// Hot Pot Roast: Cranberry Sauce 1 Roots Platter 1 Stuffing 1 Onion 1
							{
								ModEntry.ObjectPrefix + "roast",
								"238 1 244 1 239 1"
								+ $" {ModEntry.JsonAssets.GetObjectId(ModEntry.ObjectPrefix + "onion")} 1"
							},
							// Hunter's Plate: Potato 1 Cabbage 1 Horseradish 1
							{
								ModEntry.ObjectPrefix + "hunters",
								"192 1"
								+ $" {ModEntry.JsonAssets.GetObjectId(ModEntry.ObjectPrefix + "cabbage")} 1"
								+ " 16 1"
							},
							// Kebab: Tortilla 1 Tomato 1 Onion 1
							{
								ModEntry.ObjectPrefix + "kebab",
								"229 1 256 1"
								+ $" {ModEntry.JsonAssets.GetObjectId(ModEntry.ObjectPrefix + "cabbage")} 1"
							},
							// Onion Soup: Onion 1 Garlic 1 Cheese 1
							{
								ModEntry.ObjectPrefix + "onionsoup",
								$"{ModEntry.JsonAssets.GetObjectId(ModEntry.ObjectPrefix + "onion")} 1"
								+ " 248 1 424 1"
							},
							// Pineapple Skewers: Pineapple 1 Onion 1 Eggplant 1
							{
								ModEntry.ObjectPrefix + "skewers",
								"832 1"
								+ $" {ModEntry.JsonAssets.GetObjectId(ModEntry.ObjectPrefix + "onion")} 1"
								+ " 272 1"
							},
							// Redberry Pie: Flour 1 Sugar 1 Redberries 3
							{
								ModEntry.ObjectPrefix + "redberrypie",
								"246 1 245 1"
								+ $" {ModEntry.JsonAssets.GetObjectId(ModEntry.ObjectPrefix + "redberry")} 3"
							},
							// Tropical Salad: Pineapple 1 Apple 1 Watermelon 1
							{
								ModEntry.ObjectPrefix + "tropicalsalad",
								"832 1 613 1"
								//+ $" {ModEntry.JsonAssets.GetObjectId(ModEntry.ObjectPrefix + "watermelon")} 1"
								+ " 637 1" // pomegranate
							},
						};
						foreach (var recipe in recipeData)
							data[recipe.Key] = ModEntry.UpdateEntry(data[recipe.Key], new[] { recipe.Value });
					}
					
					foreach (var recipe in data.ToDictionary(pair => pair.Key, pair => pair.Value))
					{
						var recipeSplit = data[recipe.Key].Split('/');

						// Remove Oil from all cooking recipes in the game
						var ingredients = recipeSplit[0].Split(' ');
						if (!ingredients.Contains("247"))
							continue;

						recipeSplit[0] = ModEntry.UpdateEntry(recipeSplit[0],
							ingredients.Where((ingredient, i) => 
								ingredient != "247" && (i <= 0 || ingredients[i - 1] != "247")).ToArray(), 
							false, true, 0, ' ');
						data[recipe.Key] = ModEntry.UpdateEntry(data[recipe.Key], recipeSplit, false, true);
					}

					asset.AsDictionary<string, string>().ReplaceWith(data);

					if (ModEntry.PrintRename)
						Log.D(data.Where(pair => recipeData.ContainsKey(pair.Key))
								.Aggregate($"Edited {asset.AssetName}:", (s, pair) => $"{s}\n{pair.Key}: {pair.Value}"),
							ModEntry.Instance.Config.DebugMode);
				}
				catch (Exception e) when (e is ArgumentException || e is NullReferenceException || e is KeyNotFoundException)
				{
					Log.D($"Did not patch {asset.AssetName}: {(!Config.DebugMode ? e.Message : e.ToString())}",
						Config.DebugMode);
				}

				return;
			}

			if (asset.AssetNameEquals(@"Data/mail"))
			{
				var data = asset.AsDictionary<string, string>().Data;
				data.Add(ModEntry.MailCookbookUnlocked, i18n.Get("mail.cookbook_unlocked"));

				// lol pan
				var whoops = "Umm, hello @."
						+ $"^There was a mix-up at the forge with your {i18n.Get("menu.cooking_equipment.name")}."
							+ $" This is a bit embarrassing, so I'll return your materials as an apology."
						+ "^Come back to the shop and we'll see about getting you that upgrade."
						+ "^ - Clint, the blacksmith"
					+ "^^^                     $ 1000g"
					+ " %item object 334 5 %% [#] Love of Cooking Meta Menu Mix-Up";
				data.Add(ModEntry.MailFryingPanWhoops, whoops);

				asset.ReplaceWith(data);

				return;
			}

			if (asset.AssetNameEquals(@"Data/ObjectInformation"))
			{
				// Edit fields of vanilla objects to revalue and recategorise some produce

				if (ModEntry.JsonAssets == null || Game1.currentLocation == null)
					return;

				var data = asset.AsDictionary<int, string>().Data;

				// Add localised names and descriptions for new objects
				foreach (var pair in data.Where(pair => pair.Value.Split('/') is string[] split && split[0].StartsWith(ModEntry.ObjectPrefix)).ToList())
				{
					var name = pair.Value.Split('/')[0].Split(new [] { '.' }, 3);
					data[pair.Key] = ModEntry.UpdateEntry(data[pair.Key],
						new[] { i18n.Get($"item.{name[2]}.name").ToString(),
							i18n.Get($"item.{name[2]}.description").ToString() },
						false, false, 4);
					if (ModEntry.PrintRename)
						Log.D($"Named {name[2]} ({i18n.Get($"item.{name[2]}.name")})", ModEntry.Instance.Config.DebugMode);
				}
				asset.AsDictionary<int, string>().ReplaceWith(data);

				if (!Config.AddRecipeRebalancing)
				{
					Log.D($"Did not edit {asset.AssetName}: New recipe scaling is disabled in config file.",
						Config.DebugMode);
					return;
				}

				try
				{
					var objectData = new Dictionary<int, string[]>
					{
						{206, new[] {null, null, "45"}}, // Pizza
						{220, new[] {null, null, "60"}}, // Chocolate Cake
						{221, new[] {null, null, "75"}}, // Pink Cake
						{419, new[] {null, "220", "-300", "Basic -26"}}, // Vinegar
						{247, new[] {null, null, "-300", "Basic -26", null, i18n.Get("item.oil.description")}}, // Oil
						{432, new[] {null, null, "-300", null, null, i18n.Get("item.truffleoil.description")}}, // Truffle Oil
						//{ModEntry.JsonAssets.GetObjectId(ObjectPrefix + "sugarcane"), new[] {null, null, null, "Basic"}},
					};
					
					// Apply above recipe changes
					foreach (var obj in objectData.Where(o => !ModEntry.FoodsThatGiveLeftovers.Contains(data[o.Key].Split('/')[0])))
						data[obj.Key] = ModEntry.UpdateEntry(data[obj.Key], obj.Value);

					if (Config.AddRecipeRebalancing)
						RebuildBuffs(ref data);

					asset.AsDictionary<int, string>().ReplaceWith(data);

					if (ModEntry.PrintRename)
						Log.D($"Edited {asset.AssetName}:" + data.Where(pair => objectData.ContainsKey(pair.Key))
								.Aggregate("", (s, pair) => $"{s}\n{pair.Key}: {pair.Value}"),
							ModEntry.Instance.Config.DebugMode);
				}
				catch (Exception e) when (e is ArgumentException || e is NullReferenceException || e is KeyNotFoundException)
				{
					Log.D($"Did not patch {asset.AssetName}: {(!Config.DebugMode ? e.Message : e.ToString())}",
						Config.DebugMode);
				}

				return;
			}

			if (asset.AssetNameEquals(@"Data/Monsters"))
			{
				if (ModEntry.JsonAssets == null || Game1.currentLocation == null)
					return;
				if (!ModEntry.Instance.Config.AddNewCropsAndStuff)
				{
					Log.D($"Did not edit {asset.AssetName}: New crops are disabled in config file.",
						Config.DebugMode);
					return;
				}

				if (!ModEntry.RedberriesEnabled)
				{
					Log.D($"Did not edit {asset.AssetName}: Redberries are disabled in system.",
						Config.DebugMode);
					return;
				}

				try
				{
					var data = asset.AsDictionary<string, string>().Data;
					var monsterData = new Dictionary<string, string[]>
					{
						{"Shadow Shaman", new[] {$"{ModEntry.JsonAssets.GetObjectId(ModEntry.ObjectPrefix + "redberry_seeds")} .0035"
						                         + (ModEntry.NettlesEnabled ? $" {ModEntry.JsonAssets.GetObjectId(ModEntry.ObjectPrefix + "nettles")} .05" : "")}},
						{"Wilderness Golem", new[] {$"{ModEntry.JsonAssets.GetObjectId(ModEntry.ObjectPrefix + "redberry_seeds")} .0065"}},
						{"Mummy", new[] {$"{ModEntry.JsonAssets.GetObjectId(ModEntry.ObjectPrefix + "redberry_seeds")} .0022"}},
						{"Pepper Rex", new[] {$"{ModEntry.JsonAssets.GetObjectId(ModEntry.ObjectPrefix + "redberry_seeds")} .02"}},
					};
					foreach (var monster in monsterData)
						data[monster.Key] = ModEntry.UpdateEntry(data[monster.Key], monster.Value, append: true);

					asset.AsDictionary<string, string>().ReplaceWith(data);

					if (ModEntry.PrintRename)
						Log.D($"Edited {asset.AssetName}:" + data.Where(pair => monsterData.ContainsKey(pair.Key))
								.Aggregate("", (s, pair) => $"{s}\n{pair.Key}: {pair.Value}"),
							ModEntry.Instance.Config.DebugMode);
				}
				catch (Exception e) when (e is ArgumentException || e is NullReferenceException || e is KeyNotFoundException)
				{
					Log.D($"Did not patch {asset.AssetName}: {(!Config.DebugMode ? e.Message : e.ToString())}",
						Config.DebugMode);
				}

				return;
			}

			if (asset.AssetNameEquals(@"LooseSprites/Cursors"))
			{
				// Home-cook a notification icon for under the HUD money tray:

				// Prime a canvas as a clipboard to hold in sequence both a copy of the vanilla icon
				// and our custom icon to merge together into some particular open space in Cursors
				const int iconW = 11;
				const int iconH = 14;
				var data = asset.AsImage().Data;
				var canvas = new Color[iconW * iconH];
				var texture = new Texture2D(Game1.graphics.GraphicsDevice, iconW, iconH);
				var vanillaIconArea = new Rectangle(383, 493, iconW, iconH);
				var targetArea = new Rectangle(IconPosition.X, IconPosition.Y, iconW, iconH);

				// Patch in a copy of the vanilla quest log icon
				data.GetData(0, vanillaIconArea, canvas, 0, canvas.Length);
				texture.SetData(canvas);
				asset.AsImage().PatchImage(texture, null, targetArea, PatchMode.Replace);

				// Chroma-key our custom icon with colours from the vanilla icon
				var colorSampleA = canvas[iconW * 5 + 1];
				var colorSampleB = canvas[iconW * 11 + 1];

				var colorR = new Color(255, 0, 0);
				var colorC = new Color(255, 0, 255);
				var colorG = new Color(0, 255, 0);
				var colorA = new Color(0, 0, 0, 0);

				ModEntry.SpriteSheet.GetData(0, new Rectangle(0, 0, iconW, iconH), canvas, 0, canvas.Length);

				for (var i = 0; i < canvas.Length; ++i)
				{
					if (canvas[i] == colorC)
						canvas[i] = colorA;
					else if (canvas[i] == colorG)
						canvas[i] = colorSampleA;
					else if (canvas[i] == colorR)
						canvas[i] = colorSampleB;
				}

				// Patch in the custom icon over the vanilla icon copy
				texture.SetData(canvas);
				asset.AsImage().PatchImage(texture, null, targetArea, PatchMode.Overlay);

				// Patch in an alpha-shaded copy of the custom icon to use for the pulse animation
				var colorShade = new Color(0, 0, 0, 0.35f);

				for (var i = 0; i < canvas.Length; ++i)
				{
					if (canvas[i] == colorSampleB)
						canvas[i] = colorShade;
					else if (canvas[i] == colorSampleA)
						canvas[i] = colorA;
				}

				// Apply changes to the Cursors sheet
				texture.SetData(canvas);
				targetArea.X -= targetArea.Width;
				asset.AsImage().PatchImage(texture, null, targetArea, PatchMode.Overlay);
			}
			else if (asset.AssetNameEquals(@"LooseSprites/JunimoNote"))
			{
				// Add icons for a new community centre bundle
				
				if (!ModEntry.Instance.Config.AddCookingCommunityCentreBundles)
				{
					Log.D($"Did not edit {asset.AssetName}: Community centre edits are disabled in config file.",
						Config.DebugMode);
					return;
				}
				
				var sourceArea = new Rectangle(160, 208, 32 * 3, 32);
				var destArea = new Rectangle(544, 212, 32 * 3, 32);
				var destImage = asset.AsImage();
				destImage.PatchImage(ModEntry.SpriteSheet, sourceArea, destArea, PatchMode.Replace);
				asset.ReplaceWith(destImage.Data);
			}
			else if (asset.AssetNameEquals(@"Maps/Beach") && false)
			{
				//if (!Config.AddCookingQuestline)
				{
					Log.D($"Did not edit {asset.AssetName}: Cooking questline is disabled in config file.",
						Config.DebugMode);
					return;
				}

				// Add dock wares to the secret beach

				// . . .
			}
			else if (asset.AssetNameEquals(@"Maps/springobjects"))
			{
				// Patch in object icons where necessary

				if (ModEntry.JsonAssets == null)
					return;

				if (true)
					return;

				int index;
				Rectangle sourceArea, destArea;
				var destImage = asset.AsImage();

				// Pitta Bread
				index = ModEntry.JsonAssets.GetObjectId("Pitta Bread");
				if (index > 0)
				{
					sourceArea = Game1.getSourceRectForStandardTileSheet(destImage.Data, 217, 16, 16);
					destArea = Game1.getSourceRectForStandardTileSheet(destImage.Data, index, 16, 16);
					destImage.PatchImage(destImage.Data, sourceArea, destArea, PatchMode.Replace);
				}
				asset.ReplaceWith(destImage.Data);
			}
			else if (asset.AssetNameEquals(@"Maps/townInterior"))
			{
				// Patch in changes for the community centre

				if (!(Game1.currentLocation is StardewValley.Locations.CommunityCenter))
					return;

				var image = asset.AsImage();

				// Openable fridge in the kitchen
				var destArea = ModEntry.DummyOpenFridgeSpriteArea; // Target some unused area of the sheet for this location

				var sourceArea = new Rectangle(320, 224, destArea.Width, destArea.Height); // Apply base fridge sprite
				image.PatchImage(image.Data, sourceArea, destArea, PatchMode.Replace);

				sourceArea = new Rectangle(0, 192, 16, 32); // Patch in opened-door fridge sprite from mouseCursors sheet
				image.PatchImage(Game1.mouseCursors2, sourceArea, destArea, PatchMode.Overlay);

				// New star on the community centre bundle tracker wall
				if (!ModEntry.Instance.Config.AddCookingCommunityCentreBundles)
				{
					Log.D($"Did not edit {asset.AssetName}: Community centre edits are disabled in config file.",
						Config.DebugMode);
				}
				else
				{
					sourceArea = new Rectangle(370, 705, 7, 7);
					destArea = new Rectangle(380, 710, sourceArea.Width, sourceArea.Height);
					image.PatchImage(image.Data, sourceArea, destArea, PatchMode.Replace);
				}

				asset.ReplaceWith(image.Data);
			}
			else if (asset.AssetNameEquals(@"Strings/Locations"))
			{
				// Make changes to facilitate a new community centre bundle

				if (!ModEntry.Instance.Config.AddCookingCommunityCentreBundles)
				{
					Log.D($"Did not edit {asset.AssetName}: Community centre edits are disabled in config file.",
						Config.DebugMode);
					return;
				}

				var data = asset.AsDictionary<string, string>().Data;

				if (data.ContainsKey("CommunityCenter_AreaName_" + ModEntry.CommunityCentreAreaName))
					return;

				data.Add("CommunityCenter_AreaName_" + ModEntry.CommunityCentreAreaName, i18n.Get("world.community_centre.kitchen"));
				const int newJunimoLineNumber = 3;
				for (var i = newJunimoLineNumber; i < ModEntry.CommunityCentreAreaNumber - 1; ++i)
					data["CommunityCenter_AreaCompletion" + i] = data["CommunityCenter_AreaCompletion" + (i - 1)];
				
				data["CommunityCenter_AreaCompletion" + newJunimoLineNumber] = i18n.Get("world.community_centre.newjunimoline");

				asset.AsDictionary<string, string>().ReplaceWith(data);
			}
			else if (asset.AssetNameEquals(@"Strings/UI"))
			{
				// Make changes to facilitate a new community centre bundle

				if (!ModEntry.Instance.Config.AddCookingCommunityCentreBundles)
				{
					Log.D($"Did not edit {asset.AssetName}: Community centre edits are disabled in config file.",
						Config.DebugMode);
					return;
				}

				var data = asset.AsDictionary<string, string>().Data;
				data["JunimoNote_Reward" + ModEntry.CommunityCentreAreaName] = i18n.Get("world.community_centre.reward");
				asset.AsDictionary<string, string>().ReplaceWith(data);
			}
			else if (asset.AssetNameEquals(@"TileSheets/tools"))
			{
				// Patch in tool sprites for cooking equipment

				if (!ModEntry.Instance.Config.AddCookingToolProgression)
				{
					Log.D($"Did not edit {asset.AssetName}: Cooking equipment is disabled in config file.",
						Config.DebugMode);
					return;
				}

				var sourceArea = new Rectangle(192, 272, 16 * 4, 16);
				var destImage = asset.AsImage();
				var destArea = new Rectangle(272, 0, sourceArea.Width, sourceArea.Height);
				destImage.PatchImage(ModEntry.SpriteSheet, sourceArea, destArea, PatchMode.Replace);
				asset.ReplaceWith(destImage.Data);
			}
		}

		private void RebuildBuffs(ref IDictionary<int, string> data)
		{
			// Reconstruct buffs of all cooking items in the game using our ingredients-to-buffs chart
			var ingredientsChart =
				ModEntry.Instance.Helper.Content.Load<Dictionary<string, string>>($"{ModEntry.BuffDataPath}.json");
			var cookingRecipes = Game1.content.Load<Dictionary<string, string>>(@"Data/CookingRecipes");
			var keys = new int[data.Keys.Count];
			data.Keys.CopyTo(keys, 0);
			foreach (var key in keys)
			{
				var objectSplit = data[key].Split('/');
				if (!objectSplit[3].Contains("-7")
				    || !cookingRecipes.ContainsKey(objectSplit[0]) // v-- Json Assets custom recipe convention for unused fields
				    || cookingRecipes[objectSplit[0]].Split('/')[1].StartsWith("what"))
					continue;
				var ingredients = cookingRecipes[objectSplit[0]].Split('/')[0].Split(' ');
				var buffArray = new[] { "0","0","0","0","0","0","0","0","0","0","0","0","0" };
				var buffDuration = 0;

				// Populate buff values using ingredients for this object in CookingRecipes
				for (var i = 0; i < ingredients.Length; i += 2)
				{
					var o = new Object(int.Parse(ingredients[i]), 0);
					var buffSplit = ingredientsChart.ContainsKey(o.Name)
						? ingredientsChart[o.Name].Split(' ')
						: null;
					if (o.ParentSheetIndex >= 2000
						)
					{
						break;
					}
					else
					{
						var random = new Random(1337 + o.Name.GetHashCode());
						if (random.NextDouble() < 0.175f)
						{
							buffSplit = new[] {
								BuffIndex.Keys.ToArray()[random.Next(BuffIndex.Count)],
								random.Next(4).ToString()
							};
						}
					}
					if (buffSplit == null)
						continue;
					for (var j = 0; j < buffSplit.Length; j += 2)
					{
						var buffName = buffSplit[j];
						var buffValue = int.Parse(buffSplit[j + 1]);
						if (buffName == "Edibility")
						{
							objectSplit[1] =
								(int.Parse(objectSplit[1]) + o.Edibility / 8 * buffValue).ToString();
						}
						else if (ModEntry.CookingAddedLevelsEnabled && buffName == "Cooking")
						{
							// TODO: UPDATE: Cooking Skill added level buffs in foods
						}
						else
						{
							buffArray[BuffIndex[buffName]] =
								(int.Parse(buffArray[BuffIndex[buffName]])
									+ (buffName == "Energy"
										? buffValue * 10 + 20
										: buffName == "Magnetism"
											? buffValue * 16 + 16
											: buffValue)).ToString();
						}
					}
					buffDuration += 4 * o.Price + Math.Max(0, o.Edibility);
				}

				buffDuration -= (int)(int.Parse(objectSplit[2]) * 0.15f);
				buffDuration = Math.Min(1600, Math.Max(300, (buffDuration / 10) * 10));

				string[] newData;
				{
					var newBuffs = buffArray.Aggregate((entry, field)
						=> $"{entry} {field}").Remove(0, 0);
					newData = new string[9];
					objectSplit.CopyTo(newData, 0);
					newData[6] ??= "food";
					if (ModEntry.Instance.Config.AddBuffReassigning)
					{
						newData[7] = newBuffs;
					}
					newData[8] = buffDuration.ToString();
				}

				data[key] = ModEntry.UpdateEntry(data[key], newData);
			}
		}
	}
}
