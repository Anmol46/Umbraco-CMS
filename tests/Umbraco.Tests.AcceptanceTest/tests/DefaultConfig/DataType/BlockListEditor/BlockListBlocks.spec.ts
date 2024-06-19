﻿import {test} from '@umbraco/playwright-testhelpers';
import {expect} from "@playwright/test";

const blockListEditorName = 'TestBlockListEditor';
const blockListLocatorName = 'Block List';
const blockListEditorAlias = 'Umbraco.BlockList';
const blockListEditorUiAlias = 'Umb.PropertyEditorUi.BlockList';

const elementTypeName = 'BlockListElement';
const dataTypeName = 'Textstring';
const groupName = 'testGroup';

test.beforeEach(async ({umbracoUi, umbracoApi}) => {
  await umbracoApi.dataType.ensureNameNotExists(blockListEditorName);
  await umbracoUi.goToBackOffice();
  await umbracoUi.dataType.goToSettingsTreeItem('Data Types');
});

test.afterEach(async ({umbracoApi}) => {
  await umbracoApi.dataType.ensureNameNotExists(blockListEditorName);
});

test('can add a label to a block', async ({page, umbracoApi, umbracoUi}) => {
  // Arrange
  const labelText = 'ThisIsALabel';
  const textStringData = await umbracoApi.dataType.getByName(dataTypeName);
  const elementTypeId = await umbracoApi.documentType.createDefaultElementType(elementTypeName, groupName, dataTypeName, textStringData.id);
  await umbracoApi.dataType.createBlockListDataTypeWithABlock(blockListEditorName, elementTypeId);

  // Act
  await umbracoUi.dataType.goToDataType(blockListEditorName);
  await umbracoUi.dataType.goToBlockWithName(elementTypeName);
  await umbracoUi.dataType.enterBlockLabelText(labelText);
  await umbracoUi.dataType.clickSubmitButton();
  await umbracoUi.dataType.clickSaveButton();

  // Assert
  await umbracoUi.dataType.isSuccessNotificationVisible();
  const block = await umbracoApi.dataType.getByName(blockListEditorName);
  expect(await umbracoApi.dataType.doesBlockListBlockContainLabel(blockListEditorName, elementTypeId, labelText)).toBeTruthy();
});

test('can update a label for a block', async ({page, umbracoApi, umbracoUi}) => {
  // Arrange
  const labelText = 'ThisIsALabel';
  const newLabelText = 'ThisIsANewLabel';
  const textStringData = await umbracoApi.dataType.getByName(dataTypeName);
  const elementTypeId = await umbracoApi.documentType.createDefaultElementType(elementTypeName, groupName, dataTypeName, textStringData.id);
  await umbracoApi.dataType.createBlockListWithBlockWithEditorAppearance(blockListEditorName, elementTypeId, labelText);
  expect(await umbracoApi.dataType.doesBlockListBlockContainLabel(blockListEditorName, elementTypeId, labelText)).toBeTruthy();

  // Act
  await umbracoUi.dataType.goToDataType(blockListEditorName);
  await umbracoUi.dataType.goToBlockWithName(elementTypeName);
  await umbracoUi.dataType.enterBlockLabelText(newLabelText);
  await umbracoUi.dataType.clickSubmitButton();
  await umbracoUi.dataType.clickSaveButton();

  // Assert
  await umbracoUi.dataType.isSuccessNotificationVisible();
  expect(await umbracoApi.dataType.doesBlockListBlockContainLabel(blockListEditorName, elementTypeId, newLabelText)).toBeTruthy();
});

test('can remove a label from a block', async ({page, umbracoApi, umbracoUi}) => {
  // Arrange
  const labelText = 'ThisIsALabel';
  const textStringData = await umbracoApi.dataType.getByName(dataTypeName);
  const elementTypeId = await umbracoApi.documentType.createDefaultElementType(elementTypeName, groupName, dataTypeName, textStringData.id);
  await umbracoApi.dataType.createBlockListWithBlockWithEditorAppearance(blockListEditorName, elementTypeId, labelText);

  // Act
  await umbracoUi.dataType.goToDataType(blockListEditorName);
  await umbracoUi.dataType.goToBlockWithName(elementTypeName);
  await umbracoUi.dataType.enterBlockLabelText("");
  await umbracoUi.dataType.clickSubmitButton();
  await umbracoUi.dataType.clickSaveButton();

  // Assert
  expect(await umbracoApi.dataType.doesBlockListBlockContainLabel(blockListEditorName, elementTypeId, "")).toBeTruthy();
});

test('can update overlay size for a block', async ({page, umbracoApi, umbracoUi}) => {
  // Arrange
  const overlaySize = 'medium';
  const textStringData = await umbracoApi.dataType.getByName(dataTypeName);
  const elementTypeId = await umbracoApi.documentType.createDefaultElementType(elementTypeName, groupName, dataTypeName, textStringData.id);
  await umbracoApi.dataType.createBlockListWithBlockWithEditorAppearance(blockListEditorName, elementTypeId, "");

  // Act
  await umbracoUi.dataType.goToDataType(blockListEditorName);
  await umbracoUi.dataType.goToBlockWithName(elementTypeName);
  await umbracoUi.dataType.updateBlockOverlaySize(overlaySize);
  await umbracoUi.dataType.clickSubmitButton();
  await umbracoUi.dataType.clickSaveButton();

  // Assert
  await umbracoUi.dataType.isSuccessNotificationVisible();
  const blockData = await umbracoApi.dataType.getByName(blockListEditorName);
  expect(blockData.values[0].value[0].editorSize).toEqual(overlaySize);
});

test('can open content model in a block', async ({page, umbracoApi, umbracoUi}) => {
  // Arrange
  const textStringData = await umbracoApi.dataType.getByName(dataTypeName);
  const elementTypeId = await umbracoApi.documentType.createDefaultElementType(elementTypeName, groupName, dataTypeName, textStringData.id);
  await umbracoApi.dataType.createBlockListDataTypeWithABlock(blockListEditorName, elementTypeId);

  // Act
  await umbracoUi.dataType.goToDataType(blockListEditorName);
  await umbracoUi.dataType.goToBlockWithName(elementTypeName);
  await umbracoUi.dataType.openBlockContentModel();

  // Assert
  await umbracoUi.dataType.isElementWorkspaceOpenInBlock(elementTypeName);
});

// There is currently frontend issues
test.skip('can remove a content model from a block', async ({page, umbracoApi, umbracoUi}) => {
  // Arrange
  const textStringData = await umbracoApi.dataType.getByName(dataTypeName);
  const elementTypeId = await umbracoApi.documentType.createDefaultElementType(elementTypeName, groupName, dataTypeName, textStringData.id);
  await umbracoApi.dataType.createBlockListDataTypeWithABlock(blockListEditorName, elementTypeId);

  // Act
  await umbracoUi.dataType.goToDataType(blockListEditorName);
  await umbracoUi.dataType.goToBlockWithName(elementTypeName);
  await page.pause();
  await umbracoUi.dataType.removeBlockContentModel();
  await umbracoUi.dataType.clickConfirmRemoveButton();
  await umbracoUi.dataType.clickSubmitButton();
  await umbracoUi.dataType.clickSaveButton();

  // Assert
  await umbracoUi.dataType.isSuccessNotificationVisible();
  const blockData = await umbracoApi.dataType.getByName(blockListEditorName);

  console.log(blockData);
  console.log(blockData.values[0]);
  await page.pause();

});

test('can add a settings model to a block', async ({page, umbracoApi, umbracoUi}) => {
  // Arrange
  const textStringData = await umbracoApi.dataType.getByName(dataTypeName);
  const contentElementTypeId = await umbracoApi.documentType.createDefaultElementType(elementTypeName, groupName, dataTypeName, textStringData.id);
  const secondElementName = 'SecondElementTest';
  const settingsElementTypeId = await umbracoApi.documentType.createDefaultElementType(secondElementName, groupName, dataTypeName, textStringData.id);
  await umbracoApi.dataType.createBlockListDataTypeWithABlock(blockListEditorName, contentElementTypeId);

  // Act
  await umbracoUi.dataType.goToDataType(blockListEditorName);
  await umbracoUi.dataType.goToBlockWithName(elementTypeName);
  await umbracoUi.dataType.addBlockSettingsModel(secondElementName);
  await umbracoUi.dataType.clickSubmitButton();
  await umbracoUi.dataType.clickSaveButton();

  // Assert
  await umbracoUi.dataType.isSuccessNotificationVisible();
  expect(await umbracoApi.dataType.doesBlockListEditorContainBlocksWithSettingsTypeIds(blockListEditorName, [settingsElementTypeId])).toBeTruthy();
});

test('can remove a settings model from a block', async ({page, umbracoApi, umbracoUi}) => {
  // Arrange
  const textStringData = await umbracoApi.dataType.getByName(dataTypeName);
  const contentElementTypeId = await umbracoApi.documentType.createDefaultElementType(elementTypeName, groupName, dataTypeName, textStringData.id);
  const secondElementName = 'SecondElementTest';
  const settingsElementTypeId = await umbracoApi.documentType.createDefaultElementType(secondElementName, groupName, dataTypeName, textStringData.id);
  await umbracoApi.dataType.createBlockListDataTypeWithContentAndSettingsElementType(blockListEditorName, contentElementTypeId, settingsElementTypeId);
  expect(await umbracoApi.dataType.doesBlockListEditorContainBlocksWithSettingsTypeIds(blockListEditorName, [settingsElementTypeId])).toBeTruthy();

  // Act
  await umbracoUi.dataType.goToDataType(blockListEditorName);
  await umbracoUi.dataType.goToBlockWithName(elementTypeName);
  await umbracoUi.dataType.removeBlockSettingsModel();
  await umbracoUi.dataType.clickConfirmRemoveButton();
  await umbracoUi.dataType.clickSubmitButton();
  await umbracoUi.dataType.clickSaveButton();

  // Assert
  await umbracoUi.dataType.isSuccessNotificationVisible();
  expect(await umbracoApi.dataType.doesBlockListEditorContainBlocksWithSettingsTypeIds(blockListEditorName, [settingsElementTypeId])).toBeFalsy();
});

test('can add a background color to a block', async ({page, umbracoApi, umbracoUi}) => {
  // Arrange
  const backgroundColor = 'red';
  const textStringData = await umbracoApi.dataType.getByName(dataTypeName);
  const contentElementTypeId = await umbracoApi.documentType.createDefaultElementType(elementTypeName, groupName, dataTypeName, textStringData.id);
  await umbracoApi.dataType.createBlockListDataTypeWithABlock(blockListEditorName, contentElementTypeId);

  // Act
  await umbracoUi.dataType.goToDataType(blockListEditorName);
  await umbracoUi.dataType.goToBlockWithName(elementTypeName);
  await umbracoUi.dataType.enterBlockBackgroundColor(backgroundColor);
  await umbracoUi.dataType.clickSubmitButton();
  await umbracoUi.dataType.clickSaveButton();

  // Assert
  await umbracoUi.dataType.isSuccessNotificationVisible();
  const blockData = await umbracoApi.dataType.getByName(blockListEditorName);
  expect(blockData.values[0].value[0].backgroundColor).toEqual(backgroundColor);
});

test('can update a background color for a block', async ({page, umbracoApi, umbracoUi}) => {
  // Arrange
  const backgroundColor = 'red';
  const newBackgroundColor = 'blue';
  const textStringData = await umbracoApi.dataType.getByName(dataTypeName);
  const contentElementTypeId = await umbracoApi.documentType.createDefaultElementType(elementTypeName, groupName, dataTypeName, textStringData.id);
  await umbracoApi.dataType.createBlockListWithBlockWithCatalogueAppearance(blockListEditorName, contentElementTypeId, backgroundColor);
  let blockData = await umbracoApi.dataType.getByName(blockListEditorName);
  expect(blockData.values[0].value[0].backgroundColor).toEqual(backgroundColor);

  // Act
  await umbracoUi.dataType.goToDataType(blockListEditorName);
  await umbracoUi.dataType.goToBlockWithName(elementTypeName);
  await umbracoUi.dataType.enterBlockBackgroundColor(newBackgroundColor);
  await umbracoUi.dataType.clickSubmitButton();
  await umbracoUi.dataType.clickSaveButton();

  // Assert
  await umbracoUi.dataType.isSuccessNotificationVisible();
  blockData = await umbracoApi.dataType.getByName(blockListEditorName);
  expect(blockData.values[0].value[0].backgroundColor).toEqual(newBackgroundColor);
});

test('can delete a background color from a block', async ({page, umbracoApi, umbracoUi}) => {
  // Arrange
  const backgroundColor = 'red';
  const textStringData = await umbracoApi.dataType.getByName(dataTypeName);
  const contentElementTypeId = await umbracoApi.documentType.createDefaultElementType(elementTypeName, groupName, dataTypeName, textStringData.id);
  await umbracoApi.dataType.createBlockListWithBlockWithCatalogueAppearance(blockListEditorName, contentElementTypeId, backgroundColor);
  let blockData = await umbracoApi.dataType.getByName(blockListEditorName);
  expect(blockData.values[0].value[0].backgroundColor).toEqual(backgroundColor);

  // Act
  await umbracoUi.dataType.goToDataType(blockListEditorName);
  await umbracoUi.dataType.goToBlockWithName(elementTypeName);
  await umbracoUi.dataType.enterBlockBackgroundColor("");
  await umbracoUi.dataType.clickSubmitButton();
  await umbracoUi.dataType.clickSaveButton();

  // Assert
  await umbracoUi.dataType.isSuccessNotificationVisible();
  blockData = await umbracoApi.dataType.getByName(blockListEditorName);
  expect(blockData.values[0].value[0].backgroundColor).toEqual("");
});

test('can add a icon color to a block', async ({page, umbracoApi, umbracoUi}) => {
// Arrange
  const iconColor = 'red';
  const textStringData = await umbracoApi.dataType.getByName(dataTypeName);
  const contentElementTypeId = await umbracoApi.documentType.createDefaultElementType(elementTypeName, groupName, dataTypeName, textStringData.id);
  await umbracoApi.dataType.createBlockListDataTypeWithABlock(blockListEditorName, contentElementTypeId);

  // Act
  await umbracoUi.dataType.goToDataType(blockListEditorName);
  await umbracoUi.dataType.goToBlockWithName(elementTypeName);
  await umbracoUi.dataType.enterBlockIconColor(iconColor);
  await umbracoUi.dataType.clickSubmitButton();
  await umbracoUi.dataType.clickSaveButton();

  // Assert
  await umbracoUi.dataType.isSuccessNotificationVisible();
  const blockData = await umbracoApi.dataType.getByName(blockListEditorName);
  expect(blockData.values[0].value[0].iconColor).toEqual(iconColor);
});

test('can update a icon color for a block', async ({page, umbracoApi, umbracoUi}) => {
  // Arrange
  const iconColor = 'red';
  const newIconColor = 'blue';
  const textStringData = await umbracoApi.dataType.getByName(dataTypeName);
  const contentElementTypeId = await umbracoApi.documentType.createDefaultElementType(elementTypeName, groupName, dataTypeName, textStringData.id);
  await umbracoApi.dataType.createBlockListWithBlockWithCatalogueAppearance(blockListEditorName, contentElementTypeId, "", iconColor);
  let blockData = await umbracoApi.dataType.getByName(blockListEditorName);
  expect(blockData.values[0].value[0].iconColor).toEqual(iconColor);

  // Act
  await umbracoUi.dataType.goToDataType(blockListEditorName);
  await umbracoUi.dataType.goToBlockWithName(elementTypeName);
  await umbracoUi.dataType.enterBlockIconColor(newIconColor);
  await umbracoUi.dataType.clickSubmitButton();
  await umbracoUi.dataType.clickSaveButton();

  // Assert
  await umbracoUi.dataType.isSuccessNotificationVisible();
  blockData = await umbracoApi.dataType.getByName(blockListEditorName);
  expect(blockData.values[0].value[0].iconColor).toEqual(newIconColor);
});

test('can delete a icon color from a block', async ({page, umbracoApi, umbracoUi}) => {
  // Arrange
  const iconColor = 'red';
  const textStringData = await umbracoApi.dataType.getByName(dataTypeName);
  const contentElementTypeId = await umbracoApi.documentType.createDefaultElementType(elementTypeName, groupName, dataTypeName, textStringData.id);
  await umbracoApi.dataType.createBlockListWithBlockWithCatalogueAppearance(blockListEditorName, contentElementTypeId, "", iconColor);
  let blockData = await umbracoApi.dataType.getByName(blockListEditorName);
  expect(blockData.values[0].value[0].iconColor).toEqual(iconColor);

  // Act
  await umbracoUi.dataType.goToDataType(blockListEditorName);
  await umbracoUi.dataType.goToBlockWithName(elementTypeName);
  await umbracoUi.dataType.enterBlockIconColor("");
  await umbracoUi.dataType.clickSubmitButton();
  await umbracoUi.dataType.clickSaveButton();

  // Assert
  await umbracoUi.dataType.isSuccessNotificationVisible();
  blockData = await umbracoApi.dataType.getByName(blockListEditorName);
  expect(blockData.values[0].value[0].iconColor).toEqual("");
});

test('can add a custom stylesheet to a block', async ({page, umbracoApi, umbracoUi}) => {

});

test('can update a custom stylesheet for a block', async ({page, umbracoApi, umbracoUi}) => {

});

test('can delete a custom stylesheet from a block', async ({page, umbracoApi, umbracoUi}) => {
});


test('can enable hide content editor in a block', async ({page, umbracoApi, umbracoUi}) => {
});

test('can disable hide content editor in a block', async ({page, umbracoApi, umbracoUi}) => {
});



