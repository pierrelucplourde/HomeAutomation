



-- -----------------------------------------------------------
-- Entity Designer DDL Script for MySQL Server 4.1 and higher
-- -----------------------------------------------------------
-- Date Created: 08/18/2014 21:11:24
-- Generated from EDMX file: D:\Documents\Programmation\HomeAutomation\HomeAutomation\WebService\HomeAutomation.WebPortal\Models\Entities.edmx
-- Target version: 3.0.0.0
-- --------------------------------------------------


-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- NOTE: if the constraint does not exist, an ignorable error will be reported.
-- --------------------------------------------------

--    ALTER TABLE `ComponentHistoricalData` DROP CONSTRAINT `FK_ComponentComponentHistoricalData`;
--    ALTER TABLE `Components` DROP CONSTRAINT `FK_DeviceComponent`;

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------
SET foreign_key_checks = 0;
    DROP TABLE IF EXISTS `Users`;
    DROP TABLE IF EXISTS `Devices`;
    DROP TABLE IF EXISTS `Components`;
    DROP TABLE IF EXISTS `ComponentHistoricalData`;
SET foreign_key_checks = 1;

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

CREATE TABLE `Users`(
	`Id` int NOT NULL AUTO_INCREMENT UNIQUE);

ALTER TABLE `Users` ADD PRIMARY KEY (Id);




CREATE TABLE `Devices`(
	`Id` int NOT NULL AUTO_INCREMENT UNIQUE, 
	`Name` longtext NOT NULL);

ALTER TABLE `Devices` ADD PRIMARY KEY (Id);




CREATE TABLE `Components`(
	`Id` int NOT NULL AUTO_INCREMENT UNIQUE, 
	`DeviceId` int NOT NULL);

ALTER TABLE `Components` ADD PRIMARY KEY (Id);




CREATE TABLE `ComponentHistoricalData`(
	`TimeStamp` datetime NOT NULL, 
	`ComponentId` int NOT NULL);

ALTER TABLE `ComponentHistoricalData` ADD PRIMARY KEY (TimeStamp, ComponentId);






-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on `ComponentId` in table 'ComponentHistoricalData'

ALTER TABLE `ComponentHistoricalData`
ADD CONSTRAINT `FK_ComponentComponentHistoricalData`
    FOREIGN KEY (`ComponentId`)
    REFERENCES `Components`
        (`Id`)
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_ComponentComponentHistoricalData'

CREATE INDEX `IX_FK_ComponentComponentHistoricalData` 
    ON `ComponentHistoricalData`
    (`ComponentId`);

-- Creating foreign key on `DeviceId` in table 'Components'

ALTER TABLE `Components`
ADD CONSTRAINT `FK_DeviceComponent`
    FOREIGN KEY (`DeviceId`)
    REFERENCES `Devices`
        (`Id`)
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_DeviceComponent'

CREATE INDEX `IX_FK_DeviceComponent` 
    ON `Components`
    (`DeviceId`);

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------
